using Replay.Models;
using Replay.Data;
using Replay.Middleware;
using Microsoft.AspNetCore.Identity;
using Replay.Services;
using Replay.Repositories;
using Microsoft.Extensions.Hosting.Internal;


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile("appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddDbContext<SecurityDbContext>();
builder.Services.AddDbContext<ProcessDBContext>();
builder.Services.AddDbContext<OperationDBContext>();

builder.Services.AddIdentityCore<User>(o => {
    o.SignIn.RequireConfirmedEmail = true;
}).AddRoles<Role>().AddEntityFrameworkStores<ApplicationDbContext>().AddSignInManager().AddDefaultTokenProviders();

builder.Services.AddAuthentication(o => {
    o.DefaultScheme = IdentityConstants.ApplicationScheme;      
    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    }).AddIdentityCookies();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.ReturnUrlParameter = "";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Dashboard";
    options.SlidingExpiration = true;
});

builder.Services.AddTransient<EmailSenderService>();
builder.Services.AddTransient<ProcessConverterService>();
builder.Services.AddScoped<SecurityRepository>();
builder.Services.AddScoped<OperationRepository>();
builder.Services.AddScoped<ProcessRepository>();
builder.Services.AddScoped<JsonImportExport>();

// Add UserRepository registration
builder.Services.AddScoped<UserRepository>();


builder.Services.AddControllersWithViews();


// Configure the database context to use SQLite


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var json = scope.ServiceProvider.GetRequiredService<JsonImportExport>();
    json.importData();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseInitialMiddleware();


// Register the ApplicationStopping event
app.Lifetime.ApplicationStopping.Register(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        var jsonImportExport = scope.ServiceProvider.GetRequiredService<JsonImportExport>();
        jsonImportExport.exportData();
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();

namespace Replay //für E2E Test
{
    public partial class Program { }
}

