
using Replay.Models;
using Microsoft.AspNetCore.Identity;


namespace Replay.Middleware;
/// <summary>This middleware checks, if any user exists. If no user exists, it will redirect to the initialregister</summary>
/// <author>Daniel Feustel</author>
public class InitialUseMiddleware
{
    private readonly RequestDelegate _next;
    
    private readonly IServiceScopeFactory _scopeFactory;
    public InitialUseMiddleware(IServiceScopeFactory scopeFactory, RequestDelegate next)
    {   
        _scopeFactory = scopeFactory;
        _next = next;
    }
    /// <summary>Middleware</summary>
    /// <param name="context">Encapsulates all HTTP-specific information about an individual HTTP request</param>
    public async Task InvokeAsync(HttpContext context)
    {

        using var scope = _scopeFactory.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        if (!userManager.Users.Any() && !context.Request.Path.StartsWithSegments("/initial"))
        {   
            if (!roleManager.Roles.Any()) {
                await roleManager.CreateAsync(new Role() {Title = "Administrator", Description = "Admin Kann Alles", Name = "Administrator"});
                await roleManager.CreateAsync(new Role() {Title = "IT", Description = "Einmal runter- und hochfahren", Name = "IT"});
                await roleManager.CreateAsync(new Role() {Title = "Backoffice", Description = "Ich hab dein Rücken", Name = "Backoffice"});
                await roleManager.CreateAsync(new Role() {Title = "Geschäftsleitung", Description = "I am da Boss", Name = "Geschäftsleitung"});
                await roleManager.CreateAsync(new Role() {Title = "Personal", Description = "Nimms nicht persönlich", Name = "Personal"});
            }
            context.Response.Redirect("/Initial");
            return;
        }

        
        await _next(context);

    }
        
    }

    


public static class InitialUseMiddlewareExtension
{
    public static IApplicationBuilder UseInitialMiddleware(
        this IApplicationBuilder builder)
    {   
        return builder.UseMiddleware<InitialUseMiddleware>();
    }
}