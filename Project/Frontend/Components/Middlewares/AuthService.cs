using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

public class AuthService
{
    private readonly ILocalStorageService _localStorage;
    private readonly NavigationManager _navigationManager;

    public AuthService(ILocalStorageService localStorage, NavigationManager navigationManager)
    {
        _localStorage = localStorage;
        _navigationManager = navigationManager;
    }

    public async Task<bool> IsAuthenticated()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        return !string.IsNullOrEmpty(token);
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _navigationManager.NavigateTo("/", forceLoad: true);
    }
}
