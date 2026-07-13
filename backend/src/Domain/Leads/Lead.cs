using System.Text.RegularExpressions;

namespace LeadEnrichment.Domain.Leads;

public sealed partial class Lead
{
    private Lead()
    {
        // EF Core materialization constructor
    }

    public Guid Id { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public LeadStatus Status { get; private set; }
    public int Score { get; private set; }
    public string ContactName { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string Industry { get; private set; } = string.Empty;
    public DateTimeOffset? EnrichedAt { get; private set; }

    public static Lead Create(
        string companyName,
        string contactName,
        string contactEmail,
        string industry,
        int score)
    {
        if (string.IsNullOrWhiteSpace(companyName))
        {
            throw new LeadValidationException("CompanyName darf nicht leer sein.");
        }

        if (string.IsNullOrWhiteSpace(contactName))
        {
            throw new LeadValidationException("ContactName darf nicht leer sein.");
        }

        if (string.IsNullOrWhiteSpace(industry))
        {
            throw new LeadValidationException("Industry darf nicht leer sein.");
        }

        if (string.IsNullOrWhiteSpace(contactEmail))
        {
            throw new LeadValidationException("ContactEmail muss ein gültiges E-Mail-Format haben.");
        }

        var trimmedCompanyName = companyName.Trim();
        var trimmedContactName = contactName.Trim();
        var trimmedContactEmail = contactEmail.Trim().ToLowerInvariant();
        var trimmedIndustry = industry.Trim();

        if (!EmailRegex().IsMatch(trimmedContactEmail))
        {
            throw new LeadValidationException("ContactEmail muss ein gültiges E-Mail-Format haben.");
        }

        if (score is < 0 or > 100)
        {
            throw new LeadValidationException("Score muss zwischen 0 und 100 liegen.");
        }

        return new Lead
        {
            Id = Guid.NewGuid(),
            CompanyName = trimmedCompanyName,
            ContactName = trimmedContactName,
            ContactEmail = trimmedContactEmail,
            Industry = trimmedIndustry,
            Score = score,
            Status = LeadStatus.New,
        };
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
