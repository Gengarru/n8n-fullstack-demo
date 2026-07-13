using LeadEnrichment.Domain.Leads;
using Microsoft.EntityFrameworkCore;

namespace LeadEnrichment.Application.Common.Persistence;

// Deliberate middle ground: Application references the EF Core *abstractions*
// package (for DbSet/IQueryable) so handlers can use LINQ-to-Entities
// directly, instead of hiding persistence behind a repository-per-query
// interface. Infrastructure's concrete DbContext (and the Npgsql provider)
// still stay out of Application entirely. This keeps the query surface
// small for a reference project without the repository-interface
// explosion a stricter Onion split would require.
public interface IAppDbContext
{
    DbSet<Lead> Leads { get; }
}
