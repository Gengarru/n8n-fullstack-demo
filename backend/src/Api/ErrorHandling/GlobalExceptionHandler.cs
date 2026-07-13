using FluentValidation;
using LeadEnrichment.Domain.Leads;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LeadEnrichment.Api.ErrorHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title) = MapException(exception);

        logger.LogError(
            exception,
            "Unhandled exception mapped to {StatusCode}: {Title}",
            statusCode,
            title);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["correlationId"] = httpContext.Items.TryGetValue("CorrelationId", out var correlationId)
                    ? correlationId
                    : null,
            },
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            options: null,
            contentType: "application/problem+json",
            cancellationToken);

        return true;
    }

    // KeyNotFoundException/a dedicated conflict exception are intentionally
    // not mapped here yet: no handler in this slice throws either one. EF
    // Core wraps ordinary DB-connectivity failures (e.g. Postgres being
    // down) in a plain System.InvalidOperationException, so matching on
    // that generic BCL type would misclassify infrastructure outages as
    // 404/409 domain conditions instead of the correct 500 — confirmed
    // live during Task 7 verification (a stopped Postgres container
    // produced a 409 instead of 500 before this was narrowed down).
    // Add typed domain exceptions (e.g. LeadNotFoundException,
    // LeadConflictException) here once a handler actually needs 404/409,
    // rather than pattern-matching on generic framework exception types.
    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        LeadValidationException => (StatusCodes.Status400BadRequest, "Ungültige Lead-Daten."),
        ValidationException => (StatusCodes.Status400BadRequest, "Ungültige Anfrage."),
        _ => (StatusCodes.Status500InternalServerError, "Ein unerwarteter Fehler ist aufgetreten."),
    };
}
