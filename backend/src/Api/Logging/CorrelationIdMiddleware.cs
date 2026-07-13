using Serilog.Context;

namespace LeadEnrichment.Api.Logging;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string HeaderName = "X-Correlation-Id";
    private const int MaxLength = 128;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = ResolveCorrelationId(context);

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private static string ResolveCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var incoming) &&
            !string.IsNullOrWhiteSpace(incoming) &&
            incoming.ToString().Length <= MaxLength)
        {
            return incoming.ToString();
        }

        return Guid.NewGuid().ToString();
    }
}
