using System.Net.Http.Headers;
using System.Security.Claims;

namespace MEMS.Admin;

// A single browser origin lets the public directory apply the same Identity session as /admin.
public static class PublicGateway
{
    public static void MapPublicSite(this WebApplication app)
    {
        app.MapGet("/api/viewer", (HttpContext context) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            return Results.Json(new { username = context.User.Identity?.Name });
        });
        app.MapGet("/api/{**path}", (HttpContext context, IHttpClientFactory clients) =>
            Forward(context, clients, "api"));
        app.MapFallback("/{**path}", (HttpContext context) =>
            Forward(context, context.RequestServices.GetRequiredService<IHttpClientFactory>(), "public"));
    }

    private static async Task Forward(HttpContext context, IHttpClientFactory clients, string service)
    {
        if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method))
        {
            context.Response.StatusCode = 405;
            return;
        }
        using var client = clients.CreateClient(service);
        // Set only the path and query. The outbound origin always comes from service discovery.
        var address = new UriBuilder(client.BaseAddress!) {
            Path = context.Request.Path.Value, Query = context.Request.QueryString.Value?.TrimStart('?')
        }.Uri;
        using var request = new HttpRequestMessage(new HttpMethod(context.Request.Method), address);
        if (service == "api")
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var token = context.User.FindFirstValue("api_token");
                if (token == null) { context.Response.StatusCode = 401; return; }
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else if (context.Request.Headers.TryGetValue("Authorization", out var supplied))
            {
                request.Headers.TryAddWithoutValidation("Authorization", supplied.ToArray());
            }
        }
        if (service == "public") request.Headers.Add("X-MEMS-Static", "1");
        using var upstream = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        context.Response.StatusCode = (int)upstream.StatusCode;
        context.Response.ContentType = upstream.Content.Headers.ContentType?.ToString();
        context.Response.ContentLength = upstream.Content.Headers.ContentLength;
        context.Response.Headers.CacheControl = service == "api" ? "private, no-store" : upstream.Headers.CacheControl?.ToString();
        if (!HttpMethods.IsHead(context.Request.Method))
            await upstream.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
}
