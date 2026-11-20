using MEMS.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDbContext<ElectionDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("elections")
        ?? throw new InvalidOperationException("The elections SQL Server connection is required.")));
builder.Services.AddIdentityCore<ElectionUser>(options => options.Stores.MaxLengthForKeys = 128)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<ElectionDb>().AddSignInManager()
    .AddClaimsPrincipalFactory<ElectionClaimsFactory>();
builder.Services.AddAuthentication(IdentityConstants.BearerScheme).AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddScoped<ElectionDirectory>();
builder.Services.AddHealthChecks().AddCheck<DatabaseHealth>("elections");

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseAuthentication();
app.Use(async (context, next) =>
{
    // Invalid tokens and removed administrators cannot fall back to the anonymous directory.
    if (context.Request.Headers.ContainsKey("Authorization") || context.User.Identity?.IsAuthenticated == true)
    {
        var signIn = context.RequestServices.GetRequiredService<SignInManager<ElectionUser>>();
        var user = await signIn.ValidateSecurityStampAsync(context.User);
        if (user == null)
        {
            await context.ChallengeAsync();
            return;
        }
        context.User = await signIn.CreateUserPrincipalAsync(user);
        if (!context.User.IsInRole(MEMS.Shared.Roles.Super) &&
            !(user.MunicipalityId.HasValue && context.User.IsInRole(MEMS.Shared.Roles.Municipal)))
        {
            await context.ForbidAsync();
            return;
        }
    }
    await next();
});
app.UseAuthorization();
app.MapControllers();
app.MapDefaultEndpoints();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ElectionDb>();
    await db.Database.MigrateAsync();
    var password = builder.Configuration["Seed:Password"] ?? (app.Environment.IsDevelopment() ? "P@$$w0rd" : null);
    await Accounts.SeedAsync(db, scope.ServiceProvider.GetRequiredService<UserManager<ElectionUser>>(),
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(), password);
    app.Logger.LogInformation("Election database initialized for {Environment}", app.Environment.EnvironmentName);
}
app.Run();

sealed class DatabaseHealth(IServiceScopeFactory scopes) : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context, CancellationToken token)
    {
        using var scope = scopes.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ElectionDb>().Database.CanConnectAsync(token)
            ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy()
            : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy();
    }
}
