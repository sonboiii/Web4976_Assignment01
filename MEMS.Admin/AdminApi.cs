using System.Net.Http.Headers;
using System.Text.Json;
using MEMS.Shared;
using Microsoft.AspNetCore.Components.Authorization;

namespace MEMS.Admin;

public sealed class AdminApi(IHttpClientFactory factory, AuthenticationStateProvider auth)
{
    public async Task<T> Get<T>(string path)
    {
        using var response = await Send(HttpMethod.Get, path);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    public async Task Write(HttpMethod method, string path, object? model = null)
    {
        using var response = await Send(method, path, model == null ? null : JsonContent.Create(model, model.GetType()));
    }

    public async Task Upload(string path, byte[] bytes)
    {
        var content = new ByteArrayContent(bytes);
        content.Headers.ContentType = new(ElectionImage.ContentType(bytes)!);
        using var response = await Send(HttpMethod.Put, path, content);
    }

    public async Task<string> Image(string path)
    {
        using var response = await Send(HttpMethod.Get, path);
        return $"data:{response.Content.Headers.ContentType?.MediaType};base64,{Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync())}";
    }

    private async Task<HttpResponseMessage> Send(HttpMethod method, string path, HttpContent? content = null)
    {
        var user = (await auth.GetAuthenticationStateAsync()).User;
        var token = user.FindFirst("api_token")?.Value ?? throw new ApplicationException("Sign in to continue.");
        using var client = factory.CreateClient("api");
        using var request = new HttpRequestMessage(method, path) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode) return response;
        using (response)
        {
            var message = (int)response.StatusCode switch
            {
                400 or 409 => await ReadMessage(response.Content),
                401 => "Your session expired. Sign out and sign in again.",
                403 or 404 => "This record is unavailable for your account.",
                413 => ElectionImage.Requirements,
                _ => "The election service is unavailable. Try again."
            };
            throw new ApplicationException(message);
        }
    }

    private static async Task<string> ReadMessage(HttpContent content)
    {
        var text = await content.ReadAsStringAsync();
        if (content.Headers.ContentType?.MediaType == "text/plain") return text;
        try
        {
            using var json = JsonDocument.Parse(text);
            var root = json.RootElement;
            if (root.ValueKind == JsonValueKind.String) return root.GetString()!;
            if (root.TryGetProperty("errors", out var errors))
                return string.Join(" ", errors.EnumerateObject().SelectMany(e => e.Value.EnumerateArray()).Select(v => v.GetString()));
            if (root.TryGetProperty("title", out var title)) return title.GetString()!;
        }
        catch (JsonException) { }
        return "Check the record and try again.";
    }
}
