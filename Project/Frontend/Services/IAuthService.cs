using System.Security.Claims;

namespace Frontend.Services;

public interface IAuthService
{
    public Task LoginAsync(string username, string password);
    public void LogoutAsync();
    public Task RegisterAsync(PersonBase user);
    public ClaimsPrincipal GetAuth();
    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; }
}