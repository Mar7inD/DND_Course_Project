global using Shared.Models;
using Shared.Auth;
using System.Text;
using Backend.Data; // For AppDbContext
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configure SQLite Database (the connection sting is inside appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteConnection")));

// Register services
builder.Services.AddScoped<PersonService>();
builder.Services.AddScoped<WasteReportService>();
builder.Services.AddScoped<Co2CalculatorService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure authentication to use JWT Bearer tokens
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
        ClockSkew = TimeSpan.Zero,
    };
});

AuthorizationPolicies.AddPolicies(builder.Services);

// Configure authorization services
builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply the authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
