using LeadEnrichment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace LeadEnrichment.IntegrationTests;

public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("lead_enrichment_test")
        .WithUsername("lead_enrichment_test")
        .WithPassword("lead_enrichment_test")
        .Build();

    public AppDbContext DbContext { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        DbContext = new AppDbContext(options);
        await DbContext.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}
