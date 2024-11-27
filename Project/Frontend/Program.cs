global using Shared.Models;
using Frontend.Auth;
using Blazored.LocalStorage;
using Frontend.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Frontend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    
builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddSingleton<TokenService>();


builder.Services.AddScoped<ChartGeneration>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddAuthentication().AddCookie(options =>
{
    options.LoginPath = "/login";
});


// Make HTTP requests to the backend
builder.Services.AddHttpClient("BackendAPI", client => 
{
    client.BaseAddress = new Uri("https://localhost:5001");
})
.AddHttpMessageHandler<AuthTokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();