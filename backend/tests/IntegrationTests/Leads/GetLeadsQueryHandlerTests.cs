using LeadEnrichment.Application.Leads.Queries.GetLeads;
using LeadEnrichment.Domain.Leads;
using Shouldly;
using Xunit;

namespace LeadEnrichment.IntegrationTests.Leads;

// Both tests share one PostgresFixture/DbContext (one container, started once
// for the whole class) with no reset between them. Test 1 only asserts against
// the 5 named seed companies (via .Where(x => seededCompanyNames.Contains(...))),
// so it's unaffected by Test 2's leftover inserted row regardless of execution
// order. Any new test added to this class must either follow the same
// narrow-assertion pattern or introduce per-test isolation (e.g. a transaction
// rollback or a fresh fixture) — don't assume the table starts empty or clean.
public sealed class GetLeadsQueryHandlerTests(PostgresFixture fixture) : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task Handle_ReturnsSeededLeads_SortedByScoreDescending()
    {
        var handler = new GetLeadsQueryHandler(fixture.DbContext);

        var result = await handler.Handle(new GetLeadsQuery(), CancellationToken.None);

        var seededCompanyNames = new[]
        {
            "Nordwind Logistik GmbH",
            "Silberpfad Media Solutions",
            "Blaufeld Systemtechnik AG",
            "Rheinallee Consulting Partners",
            "Kupferbach Elektronik KG",
        };

        var seeded = result
            .Where(x => seededCompanyNames.Contains(x.CompanyName))
            .Select(x => x.CompanyName)
            .ToList();

        seeded.ShouldBe(seededCompanyNames);
    }

    [Fact]
    public async Task Handle_WithTiedScores_SortsByCompanyNameAscending()
    {
        // Two fully synthetic leads with a matching score (100 - the maximum
        // valid Lead.Create score, and not equal to any real seed score:
        // 87/71/64/52/12) rather than reusing a real seed lead's score: this
        // keeps the test immune to LeadSeedData.cs ever changing which
        // company has which score.
        const int tiedScore = 100;

        var leadB = Lead.Create("Zzz Tiebreak Company", "Test Contact", "tiebreak-b@example.com", "Testing", tiedScore);
        var leadA = Lead.Create("Aaa Tiebreak Company", "Test Contact", "tiebreak-a@example.com", "Testing", tiedScore);

        fixture.DbContext.Leads.AddRange(leadB, leadA);
        await fixture.DbContext.SaveChangesAsync();

        var handler = new GetLeadsQueryHandler(fixture.DbContext);
        var result = await handler.Handle(new GetLeadsQuery(), CancellationToken.None);

        var tiedCompanyNames = result
            .Where(x => x.Score == tiedScore)
            .Select(x => x.CompanyName)
            .ToList();

        // "Aaa Tiebreak Company" sorts before "Zzz Tiebreak Company" alphabetically,
        // proving the ThenBy(CompanyName) tie-break actually runs, not just the
        // OrderByDescending(Score) primary sort. Order of insertion above (B then A)
        // is deliberately reversed from the expected output order, so the test can't
        // pass by coincidentally matching insertion order instead of the real sort.
        tiedCompanyNames.ShouldBe(["Aaa Tiebreak Company", "Zzz Tiebreak Company"]);
    }
}
