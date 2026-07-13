using LeadEnrichment.Application.Common.Persistence;
using LeadEnrichment.Domain.Leads;
using Microsoft.EntityFrameworkCore;

namespace LeadEnrichment.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IAppDbContext
{
    public DbSet<Lead> Leads => Set<Lead>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        LeadSeedData.Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
