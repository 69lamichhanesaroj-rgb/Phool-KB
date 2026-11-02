using Infra;
using Infra.Entities;
using LinqToDB;
using LinqToDB.DataProvider.SQLite;
using Service;
using Service.Security;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=phool.db";

// DI registrations
builder.Services.AddSingleton(new DataOptions().UseSQLite(connectionString, SQLiteProvider.Microsoft));
builder.Services.AddScoped<AppDataConnection>();
builder.Services.AddSingleton<HashingPassword>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SeedRole>();
builder.Services.AddScoped<SeedUser>();

builder.Services.AddControllers();

var app = builder.Build();

/*
 * Seed the database with roles and users.
 * Used only once when the app is run for the first time.
 */
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDataConnection>();
    db.CreateTable<Role>(statementHeader: "CREATE TABLE IF NOT EXISTS {0}");
    db.CreateTable<User>(statementHeader: "CREATE TABLE IF NOT EXISTS {0}");

    scope.ServiceProvider.GetRequiredService<SeedRole>().SeedRoles();
    scope.ServiceProvider.GetRequiredService<SeedUser>().SeedUsers();
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();