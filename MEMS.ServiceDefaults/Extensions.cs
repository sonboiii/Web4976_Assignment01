using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    public static void AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        var export = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        builder.Logging.AddOpenTelemetry(o =>
        {
            o.IncludeFormattedMessage = true;
            o.IncludeScopes = true;
            if (export) o.AddOtlpExporter();
        });
        builder.Services.AddOpenTelemetry().WithTracing(t =>
        {
            t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
            if (export) t.AddOtlpExporter();
        }).WithMetrics(m =>
        {
            m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation();
            if (export) m.AddOtlpExporter();
        });
        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http => http.AddServiceDiscovery());
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ServerErrors>();
        builder.Services.AddHealthChecks().AddCheck("live", () => HealthCheckResult.Healthy(), ["live"]);
    }

    public static void MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/alive", new() { Predicate = r => r.Tags.Contains("live") });
    }
}

internal sealed class ServerErrors(ILogger<ServerErrors> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken token)
    {
        if (exception is BadHttpRequestException badRequest)
        {
            await Results.Problem(title: "The request is invalid or exceeds the upload limit.", statusCode: badRequest.StatusCode).ExecuteAsync(context);
            return true;
        }
        logger.LogError(exception, "Unhandled error on {Method} {Path}", context.Request.Method, context.Request.Path);
        await Results.Problem(title: "The request could not be completed.", statusCode: 500).ExecuteAsync(context);
        return true;
    }
}
