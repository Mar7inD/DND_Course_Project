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

    public async Task<(bool IsAuthenticated, string? EmployeeId)> IsAuthenticatedAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var employeeId = await _localStorage.GetItemAsync<string>("employeeId");
        
        bool isAuthenticated = !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(employeeId);

        return (isAuthenticated, employeeId);
    }


    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _navigationManager.NavigateTo("/", forceLoad: true);
    }
}
