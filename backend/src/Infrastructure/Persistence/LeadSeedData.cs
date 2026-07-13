using LeadEnrichment.Domain.Leads;
using Microsoft.EntityFrameworkCore;

namespace LeadEnrichment.Infrastructure.Persistence;

public static class LeadSeedData
{
    // Intentionally bypasses Lead.Create's validation: EF Core's HasData
    // requires property-only anonymous objects built at migration-design
    // time, before any Lead instance exists to validate. These are
    // trusted, developer-authored fixture rows (not user input), and all
    // 5 satisfy Create's rules anyway — kept in sync manually.
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lead>().HasData(
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111001"),
                CompanyName = "Nordwind Logistik GmbH",
                Status = LeadStatus.Qualified,
                Score = 87,
                ContactName = "Petra Lindgren",
                ContactEmail = "p.lindgren@nordwind-logistik.example",
                Industry = "Logistik",
                EnrichedAt = (DateTimeOffset?)DateTimeOffset.Parse("2026-06-30T09:12:00Z"),
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111002"),
                CompanyName = "Blaufeld Systemtechnik AG",
                Status = LeadStatus.Enriched,
                Score = 64,
                ContactName = "Markus Vogt",
                ContactEmail = "m.vogt@blaufeld-systemtechnik.example",
                Industry = "Maschinenbau",
                EnrichedAt = (DateTimeOffset?)DateTimeOffset.Parse("2026-07-02T14:45:00Z"),
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111003"),
                CompanyName = "Rheinallee Consulting Partners",
                Status = LeadStatus.Contacted,
                Score = 52,
                ContactName = "Sabine Hoff",
                ContactEmail = "s.hoff@rheinallee-consulting.example",
                Industry = "Beratung",
                EnrichedAt = (DateTimeOffset?)DateTimeOffset.Parse("2026-06-28T11:30:00Z"),
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111004"),
                CompanyName = "Kupferbach Elektronik KG",
                Status = LeadStatus.New,
                Score = 12,
                ContactName = "Jonas Wetter",
                ContactEmail = "j.wetter@kupferbach-elektronik.example",
                Industry = "Elektronik",
                EnrichedAt = (DateTimeOffset?)null,
            },
            new
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111005"),
                CompanyName = "Silberpfad Media Solutions",
                Status = LeadStatus.Enriched,
                Score = 71,
                ContactName = "Anke Brummer",
                ContactEmail = "a.brummer@silberpfad-media.example",
                Industry = "Medien",
                EnrichedAt = (DateTimeOffset?)DateTimeOffset.Parse("2026-07-05T08:05:00Z"),
            });
    }
}
