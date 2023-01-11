using DatingApp.Api.Data;
using DatingApp.Api.Entities;
using DatingApp.Api.Extensions;
using DatingApp.Api.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(corsBuilder => corsBuilder.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
    .WithOrigins("http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsersAsync(userManager);
}
catch (Exception exc)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(exc, "An error occurred during migration");
}

app.Run();

