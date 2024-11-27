using System.Security.Claims;
using Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Auth;

public class CustomAuthProvider : AuthenticationStateProvider
{
    private readonly IAuthService authService;

    public CustomAuthProvider(IAuthService authService)
    {
        this.authService = authService;
        authService.OnAuthStateChanged += AuthStateChanged;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal principal = authService.GetAuth();

        await Task.CompletedTask;
        return new AuthenticationState(principal);
    }

    private void AuthStateChanged(ClaimsPrincipal principal)
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(principal)
            )
        );
    }
}
