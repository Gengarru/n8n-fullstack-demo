using Mediator;

namespace LeadEnrichment.Application.Leads.Queries.GetLeads;

public sealed record GetLeadsQuery : IRequest<IReadOnlyList<LeadDto>>;
