namespace LeadEnrichment.Domain.Leads;

public sealed class LeadValidationException : Exception
{
    public LeadValidationException(string message) : base(message)
    {
    }
}
