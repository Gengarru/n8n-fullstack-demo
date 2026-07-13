using Asp.Versioning;
using LeadEnrichment.Application.Leads.Queries.GetLeads;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace LeadEnrichment.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/leads")]
public sealed class LeadsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<LeadDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LeadDto>>> GetLeads(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetLeadsQuery(), cancellationToken);
        return Ok(result);
    }
}
