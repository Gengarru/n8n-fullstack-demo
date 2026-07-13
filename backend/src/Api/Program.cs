using Asp.Versioning;
using FluentValidation;
using LeadEnrichment.Api.ErrorHandling;
using LeadEnrichment.Api.Logging;
using LeadEnrichment.Application.Common.Behaviors;
using LeadEnrichment.Application.Common.Persistence;
using LeadEnrichment.Infrastructure.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext());

var appDbConnectionString = builder.Configuration.GetConnectionString("AppDb")
    ?? throw new InvalidOperationException("Connection string 'AppDb' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(appDbConnectionString));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
});

builder.Services.AddValidatorsFromAssembly(
    typeof(LeadEnrichment.Application.Leads.Queries.GetLeads.GetLeadsQuery).Assembly);

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services
    .AddHealthChecks()
    .AddNpgSql(
        appDbConnectionString,
        name: "postgres",
        tags: ["ready"]);

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseExceptionHandler();

app.MapControllers();

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false,
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
});

app.Run();
