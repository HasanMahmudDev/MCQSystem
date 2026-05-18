using MCQSystem.Application;
using MCQSystem.Application.Interfaces;
using MCQSystem.Infrastructure;
using MCQSystem.Persistence;
using MCQSystem.Shared;
using MCQSystem.WebUI.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", AuthorizationPolicies.AdminOnly);
    options.Conventions.AuthorizeFolder("/Student", AuthorizationPolicies.StudentOrAdmin);
});

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.Name = "MCQSystem.Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireRole(AppRoles.Admin));
    options.AddPolicy(AuthorizationPolicies.StudentOrAdmin, policy => policy.RequireRole(AppRoles.Student, AppRoles.Admin));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var mongoSettings = scope.ServiceProvider.GetRequiredService<IOptions<MongoDbSettings>>();

    try
    {
        await MongoDbStartup.EnsureAvailableAsync(mongoSettings, logger);
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (InvalidOperationException ex)
    {
        logger.LogCritical(ex, "Application startup failed because MongoDB is unavailable.");
        Console.Error.WriteLine(ex.Message);
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuditTrailMiddleware>();
app.MapRazorPages();

app.Run();
