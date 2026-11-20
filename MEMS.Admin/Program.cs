using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using MEMS.Admin;
using MEMS.Admin.Components;
using MEMS.Shared;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.BearerToken;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("mems-admin").AddCookie("mems-admin", options =>
{
    options.Cookie.Name = "MEMS.Admin";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.LoginPath = "/admin/login";
    options.SlidingExpiration = false;
});
builder.Services.AddHttpClient("api", c => c.BaseAddress = new("http://api"));
builder.Services.AddHttpClient("public", c => c.BaseAddress = new("http://public"));
builder.Services.AddScoped<AdminApi>();
var app = builder.Build();
app.UseExceptionHandler();
app.UseWhen(c => !c.Request.Path.StartsWithSegments("/health") && !c.Request.Path.StartsWithSegments("/alive"),
    browser => browser.UseHttpsRedirection());
app.MapStaticAssets();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapPublicSite();
app.MapPost("/admin/session", async (HttpContext context, IAntiforgery antiforgery, IHttpClientFactory clients) =>
{
    if (!await antiforgery.IsRequestValidAsync(context)) return Results.BadRequest();
    var form = await context.Request.ReadFormAsync();
    using var client = clients.CreateClient("api");
    using var login = await client.PostAsJsonAsync("/api/account/login", new Credentials(form["username"].ToString(), form["password"].ToString()));
    if (!login.IsSuccessStatusCode) return Results.Redirect("/admin/login?failed=true");
    var token = (await login.Content.ReadFromJsonAsync<AccessTokenResponse>())!;
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    var user = await client.GetFromJsonAsync<Account>("/api/account/me");
    if (user == null) return Results.Redirect("/admin/login?failed=true");
    var claims = new List<Claim> { new(ClaimTypes.Name, user.Username), new(ClaimTypes.Role, user.Role),
        new("api_token", token.AccessToken) };
    if (user.MunicipalityId is int id) claims.Add(new(Roles.TenantClaim, id.ToString()));
    await context.SignInAsync("mems-admin", new ClaimsPrincipal(new ClaimsIdentity(claims, "mems-admin")),
        new AuthenticationProperties { ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn) });
    return Results.Redirect("/admin");
});
app.MapPost("/admin/logout", async (HttpContext context, IAntiforgery antiforgery) =>
{
    if (!await antiforgery.IsRequestValidAsync(context)) return Results.BadRequest();
    await context.SignOutAsync("mems-admin");
    return Results.Redirect("/admin/login");
});
app.MapDefaultEndpoints();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
