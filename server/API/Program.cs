using System.Text;
using Infra;
using Infra.Entities;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service;
using Service.Security;

var builder = WebApplication.CreateBuilder(args);

// Jwt configuration
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=phool.db";
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("Missing 'Jwt' section in configuration.");
builder.Services.AddSingleton(jwtOptions);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var clientOrigin = builder.Configuration["Cors:ClientOrigin"] ?? "http://localhost:3000";

builder.Services.AddCors(options =>
{
    options.AddPolicy("client", policy => policy.WithOrigins(clientOrigin).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddAuthorization();

// DI registrations
builder.Services.AddSingleton(new DataOptions().UseSQLite(connectionString, SQLiteProvider.Microsoft));
builder.Services.AddScoped<AppDataConnection>();
builder.Services.AddSingleton<HashingPassword>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<SeedRole>();
builder.Services.AddScoped<SeedUser>();
builder.Services.AddSingleton<TokenGen>();

builder.Services.AddControllers();

var app = builder.Build();


// Seed the database with roles and users.
// Used only once when the app is run for the first time.

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataConnection>();
    db.CreateTable<Role>(statementHeader: "CREATE TABLE IF NOT EXISTS {0}");
    db.CreateTable<User>(statementHeader: "CREATE TABLE IF NOT EXISTS {0}");
    db.Execute("CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Username ON Users(Username)");

    scope.ServiceProvider.GetRequiredService<SeedRole>().SeedRoles();
    scope.ServiceProvider.GetRequiredService<SeedUser>().SeedUsers();
}
app.UseCors("client");
app.MapControllers();

app.Run();