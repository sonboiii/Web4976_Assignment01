#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false
#:project ../MEMS.Public/MEMS.Public.csproj
using System.Net;
using System.Net.Http.Json;
using MEMS.Public;
using MEMS.Public.Pages;
using MEMS.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

var api = new PublicSlates();
var storage = new SessionChoices();
var app = Host.CreateApplicationBuilder();
app.Services.AddSingleton<IJSRuntime>(storage).AddSingleton(new HttpClient(api) { BaseAddress = new("http://fixture.invalid") });
using var host = app.Build();
await using var renderer = new HtmlRenderer(host.Services, host.Services.GetRequiredService<ILoggerFactory>());

async Task<string> Render<T>(bool administrator, string? slug = null, string? section = null) where T : IComponent
{
    RenderFragment content = builder =>
    {
        builder.OpenComponent<T>(0);
        if (slug != null) builder.AddAttribute(1, "Slug", slug);
        if (section != null) builder.AddAttribute(2, "Section", section);
        builder.CloseComponent();
    };
    var parameters = ParameterView.FromDictionary(new Dictionary<string, object?> {
        ["Name"] = "Administrator", ["Value"] = administrator, ["ChildContent"] = content
    });
    return await renderer.Dispatcher.InvokeAsync(async () =>
        (await renderer.RenderComponentAsync<CascadingValue<bool>>(parameters)).ToHtmlString());
}

void Check(bool condition, string label)
{
    if (!condition) throw new InvalidOperationException(label);
    Console.WriteLine("PASS " + label);
}

var html = await Render<CombinedGuide>(false);
Check(new[] { "Pitt Meadows", "Surrey", "Amelia Chen", "Surrey Candidate", "Mayor", "2026", "Print date" }.All(s => html.Contains(s))
    && !html.Contains("Unselected Person"), "visitor prints chosen candidates from two cities");
var personalized = api.Requests.ToArray();
storage.Empty = true;
api.Requests.Clear();
html = await Render<CombinedGuide>(false);
Check(html.Contains("Your guide is empty") && personalized.SequenceEqual(api.Requests),
    "empty and multi-city guides generate identical public API reads");

storage.Empty = false;
storage.Reads = 0;
api.Requests.Clear();
html = await Render<CombinedGuide>(true);
Check(!html.Contains("Amelia Chen") && storage.Reads == 0 && api.Requests.Count == 0,
    "administrator opening /guide reads no ballot data");
html = await Render<TenantDirectory>(true, "pitt-meadows");
Check(html.Contains("Amelia Chen") && !html.Contains("Add to guide") && !html.Contains("Remove from guide")
    && !html.Contains("Compare") && storage.Reads == 0, "administrator can browse a tenant without shortlisting controls");
html = await Render<TenantDirectory>(true, "pitt-meadows", "guide");
Check(!html.Contains("Print guide") && storage.Reads == 0, "direct tenant guide URL hides voter selections");
html = await Render<TenantDirectory>(false, "pitt-meadows");
Check(html.Contains("Remove from guide") && html.Contains("Compare"), "visitor can shortlist and compare");

sealed class SessionChoices : IJSRuntime
{
    public bool Empty;
    public int Reads;
    public ValueTask<T> InvokeAsync<T>(string method, object?[]? args)
    {
        if (method != "civic.read") throw new InvalidOperationException(method);
        Reads++;
        int[] ids = !Empty && Equals(args![1], "guide") ? [(int)args[0]! * 10] : [];
        return ValueTask.FromResult((T)(object)ids);
    }
    public ValueTask<T> InvokeAsync<T>(string method, CancellationToken token, object?[]? args) => InvokeAsync<T>(method, args);
}

sealed class PublicSlates : HttpMessageHandler
{
    private readonly Municipality[] towns = [
        new() { Id = 1, Name = "Pitt Meadows", Slug = "pitt-meadows" },
        new() { Id = 2, Name = "Surrey", Slug = "surrey" }
    ];
    public List<string> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        if (request.Method != HttpMethod.Get || request.Content != null) throw new InvalidOperationException("Unexpected write");
        var path = request.RequestUri!.PathAndQuery;
        Requests.Add(path);
        if (path == "/api/municipalities") return Response(towns);
        var parts = path.Trim('/').Split('/');
        var town = towns.Single(m => m.Slug == parts[1]);
        return parts[2] switch
        {
            "municipality" => Response(town),
            "positions" => Response(new[] { new Position { Id = town.Id, Name = "Mayor", MunicipalityId = town.Id } }),
            "candidates" => Response(new[] {
                new Candidate { Id = town.Id * 10, PositionId = town.Id, MunicipalityId = town.Id,
                    Name = town.Id == 1 ? "Amelia Chen" : "Surrey Candidate" },
                new Candidate { Id = town.Id * 10 + 1, PositionId = town.Id, MunicipalityId = town.Id, Name = "Unselected Person" }
            }),
            _ => throw new InvalidOperationException(path)
        };
    }
    private static Task<HttpResponseMessage> Response<T>(T value) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(value) });
}
