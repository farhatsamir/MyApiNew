using BlazorApp1;
using BlazorApp1.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using Serilog.Sinks.File;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("C:/Users/lenovo/Desktop/LogSamirApp/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
Log.Information("Application Started");

builder.Services.AddTransient<CookieHandler>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();

builder.Services.AddScoped(sp => (IAccountManagement)sp.GetRequiredService<AuthenticationStateProvider>());

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", "<your_jwt_token>") }
});

builder.Services.AddHttpClient("Auth", options =>
    options.BaseAddress = new Uri("https://localhost:7293")
).AddHttpMessageHandler<CookieHandler>();

await builder.Build().RunAsync();
