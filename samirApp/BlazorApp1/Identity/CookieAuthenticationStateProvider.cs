﻿using BlazorApp1.Models;
using BlazorApp1.Pages.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Login = BlazorApp1.Models.Login;

namespace BlazorApp1.Identity
{
    public class CookieAuthenticationStateProvider(IHttpClientFactory httpClientFactory) : AuthenticationStateProvider , IAccountManagement
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Auth");
        private bool _authenticated;
        private readonly ClaimsPrincipal _unauthenticated = new ClaimsPrincipal(new ClaimsIdentity());

        private readonly JsonSerializerOptions _jsonSerializerOptions = new ()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _authenticated = false;

            var user = _unauthenticated;

            try
            {
                var userResponse = await _httpClient.GetAsync("manage/info");

                userResponse.EnsureSuccessStatusCode();

                var userJson = await userResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, _jsonSerializerOptions);

                if (userInfo is not null)
                {
                    _authenticated = true;

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, userInfo.Email),
                        new(ClaimTypes.Email, userInfo.Email),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));

                     user = new ClaimsPrincipal(claimsIdentity);

                }

            }
            catch
            { 
                //Loging
            }

            return new AuthenticationState(user);
        }

        public async Task<AuthResult> LoginAsync(Login login)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync("login?useCookies=true", new
                {
                    login.Email,
                    login.Password,
                });

                if (result.IsSuccessStatusCode)
                {
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    Log.Information("User {UserId} logged in at {Time}", login);
                    return new AuthResult { Succeeded = true };
                    
                }
            }
            catch
            {

            }
            return new AuthResult
            {
                Succeeded = false,
                ErrorList = ["Invalaid Email or Password"]
            };
        }

        public async Task<AuthResult> RegisterAsync(string email, string password)
        {
            string[] deaultErrors = ["An unknown error"];
            try
            {
                var result = await _httpClient.PostAsJsonAsync("register", new
                {
                    email,
                    password,
                });

                if (result.IsSuccessStatusCode)
                {
                    return new AuthResult { Succeeded = true };
                }

                var details = await result.Content.ReadAsStringAsync();
                var problemDetails = JsonDocument.Parse(details);

                var errors = new List<string>();
                var errorList = problemDetails.RootElement.GetProperty("errors");
                foreach (var error in errorList.EnumerateObject()) 
                {
                    if(error.Value.ValueKind == JsonValueKind.String)
                    {
                        errors.Add(error.Value.GetString()!);
                    }
                    else if(error.Value.ValueKind == JsonValueKind.Array)
                    {
                        var allErrors = error.Value
                            .EnumerateArray()
                            .Select(e => e.GetString() ?? string.Empty)
                            .Where(e => !string.IsNullOrEmpty(e));

                        errors.AddRange(allErrors);

                    }
                }
                return new AuthResult
                {
                    Succeeded = false,
                    ErrorList = [..errors]
                };
            }
            catch
            {
            }

            return new AuthResult
            {
                Succeeded = false,
                ErrorList = deaultErrors
            };
        }


        public async Task LogoutAsync()
        {
            var emptyContect = new StringContent("{ }", Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("auth/logout", emptyContect);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
