namespace LeadEnrichment.Domain.Leads;

// Bewusst eigener, für dieses fiktive Szenario erfundener Status-Flow —
// KEIN Abbild des echten CRM-Workflows (siehe CLAUDE.md "keine Übernahme
// von Datenmodell/Logik aus dem eigenen CRM-Produkt"). Der echte
// CRM-Pipeline-Status (New/Connected/Accepted/.../Dismissed) ist hier
// absichtlich NICHT übernommen.
public enum LeadStatus
{
    New,
    Enriched,
    Contacted,
    Qualified,
}
