using LeadEnrichment.Domain.Leads;

namespace LeadEnrichment.Application.Leads.Queries.GetLeads;

public sealed record LeadDto(
    Guid Id,
    string CompanyName,
    LeadStatus Status,
    int Score,
    string ContactName,
    string ContactEmail,
    string Industry,
    DateTimeOffset? EnrichedAt);
