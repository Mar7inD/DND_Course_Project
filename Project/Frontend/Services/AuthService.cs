using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace Frontend.Services;

public class AuthService(HttpClient client, IJSRuntime jsRuntime, NavigationManager _navigationManager) : IAuthService
{
    
    public string Jwt { get; private set; } = "";

    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; } = null!;

    public async Task LoginAsync(string employeeId, string password)
    {
        
        PersonBaseDTO userLoginDto = new()
        {
            EmployeeId = employeeId,
            Password = password
        };

        string userAsJson = JsonSerializer.Serialize(userLoginDto);
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("https://localhost:5001/api/Auth/login", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }

        string token = responseContent;
        Jwt = token;

        await CacheTokenAsync();
        
        ClaimsPrincipal principal = await CreateClaimsPrincipal();
        
        OnAuthStateChanged.Invoke(principal);
        
    }

    private async Task<ClaimsPrincipal> CreateClaimsPrincipal()
    {
        var cachedToken = await GetTokenFromCacheAsync();
        if (string.IsNullOrEmpty(Jwt) && string.IsNullOrEmpty(cachedToken))
        {
            return new ClaimsPrincipal();
        }
        if (!string.IsNullOrEmpty(cachedToken))
        {
            Jwt = cachedToken;
        }
        if (!client.DefaultRequestHeaders.Contains("Authorization"))
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Jwt);

        IEnumerable<Claim> claims = ParseClaimsFromJwt(Jwt);

        ClaimsIdentity identity = new(claims, "jwt");

        ClaimsPrincipal principal = new(identity);
        return principal;
    }

    public async Task LogoutAsync()
    {
        await ClearTokenFromCacheAsync();
        Jwt = "";
        ClaimsPrincipal principal = new();
        OnAuthStateChanged.Invoke(principal);
        _navigationManager.NavigateTo("/", forceLoad: true);
    }

    public async Task RegisterAsync(PersonBase user)
    {
        string userAsJson = JsonSerializer.Serialize(user);
        StringContent content = new(userAsJson, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync("https://localhost:5001/api/Auth/register", content);
        string responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(responseContent);
        }
    }

    public async Task<ClaimsPrincipal> GetAuthAsync()
    {
        ClaimsPrincipal principal = await CreateClaimsPrincipal();
        return principal;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null)
        {
            throw new Exception("Failed to parse JWT claims.");
        }

        foreach (var kvp in keyValuePairs)
        {
            var key = kvp.Key;
            var value = kvp.Value.ToString();

            if (key == "role" || key == ClaimTypes.Role)
            {
                // Handle roles as an array or single value
                if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.GetString()!));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, value!));
                }
            }
            else
            {
                claims.Add(new Claim(key, value!));
            }
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }

    private async Task<string?> GetTokenFromCacheAsync()
    {
        return await jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt");
    }

    private async Task CacheTokenAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt", Jwt);
    }

    private async Task ClearTokenFromCacheAsync()
    {
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwt", "");
    }
}
