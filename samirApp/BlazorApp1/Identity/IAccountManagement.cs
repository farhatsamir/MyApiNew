using BlazorApp1.Models;

namespace BlazorApp1.Identity
{
    public interface IAccountManagement
    {
        Task<AuthResult> LoginAsync(Login login);
        Task<AuthResult> RegisterAsync(string email , string password);
        Task LogoutAsync();
    }
}
