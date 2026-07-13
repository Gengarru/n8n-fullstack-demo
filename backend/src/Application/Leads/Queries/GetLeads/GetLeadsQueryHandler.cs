using LeadEnrichment.Application.Common.Persistence;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace LeadEnrichment.Application.Leads.Queries.GetLeads;

public sealed class GetLeadsQueryHandler(IAppDbContext dbContext)
    : IRequestHandler<GetLeadsQuery, IReadOnlyList<LeadDto>>
{
    public async ValueTask<IReadOnlyList<LeadDto>> Handle(
        GetLeadsQuery request,
        CancellationToken cancellationToken)
    {
        return await dbContext.Leads
            .AsNoTracking()
            .OrderByDescending(x => x.Score)
            .ThenBy(x => x.CompanyName)
            .Select(x => new LeadDto(
                x.Id,
                x.CompanyName,
                x.Status,
                x.Score,
                x.ContactName,
                x.ContactEmail,
                x.Industry,
                x.EnrichedAt))
            .ToListAsync(cancellationToken);
    }
}
