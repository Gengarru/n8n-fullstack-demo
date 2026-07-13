using LeadEnrichment.Domain.Leads;
using Shouldly;
using Xunit;

namespace LeadEnrichment.UnitTests.Domain.Leads;

public class LeadTests
{
    private const string ValidCompanyName = "Test GmbH";
    private const string ValidContactName = "Max Mustermann";
    private const string ValidContactEmail = "max@test.example";
    private const string ValidIndustry = "IT";
    private const int ValidScore = 50;

    [Fact]
    public void Create_WithValidInput_SetsStatusToNew()
    {
        var lead = Lead.Create(ValidCompanyName, ValidContactName, ValidContactEmail, ValidIndustry, ValidScore);

        lead.Status.ShouldBe(LeadStatus.New);
    }

    [Fact]
    public void Create_WithValidInput_TrimsAndNormalizesFields()
    {
        var lead = Lead.Create("  Test GmbH  ", "  Max Mustermann  ", "  MAX@TEST.EXAMPLE  ", "  IT  ", ValidScore);

        lead.CompanyName.ShouldBe("Test GmbH");
        lead.ContactName.ShouldBe("Max Mustermann");
        lead.ContactEmail.ShouldBe("max@test.example");
        lead.Industry.ShouldBe("IT");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCompanyName_ThrowsLeadValidationException(string companyName)
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(companyName, ValidContactName, ValidContactEmail, ValidIndustry, ValidScore));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyContactName_ThrowsLeadValidationException(string contactName)
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, contactName, ValidContactEmail, ValidIndustry, ValidScore));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyIndustry_ThrowsLeadValidationException(string industry)
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, ValidContactName, ValidContactEmail, industry, ValidScore));
    }

    [Fact]
    public void Create_WithNullCompanyName_ThrowsLeadValidationException()
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(null!, ValidContactName, ValidContactEmail, ValidIndustry, ValidScore));
    }

    [Fact]
    public void Create_WithNullContactName_ThrowsLeadValidationException()
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, null!, ValidContactEmail, ValidIndustry, ValidScore));
    }

    [Fact]
    public void Create_WithNullContactEmail_ThrowsLeadValidationException()
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, ValidContactName, null!, ValidIndustry, ValidScore));
    }

    [Fact]
    public void Create_WithNullIndustry_ThrowsLeadValidationException()
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, ValidContactName, ValidContactEmail, null!, ValidScore));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("kein-at-zeichen")]
    [InlineData("kein-punkt@example")]
    [InlineData("@example.com")]
    public void Create_WithInvalidContactEmail_ThrowsLeadValidationException(string contactEmail)
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, ValidContactName, contactEmail, ValidIndustry, ValidScore));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Create_WithBoundaryScore_Succeeds(int score)
    {
        var lead = Lead.Create(ValidCompanyName, ValidContactName, ValidContactEmail, ValidIndustry, score);

        lead.Score.ShouldBe(score);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Create_WithOutOfRangeScore_ThrowsLeadValidationException(int score)
    {
        Should.Throw<LeadValidationException>(() =>
            Lead.Create(ValidCompanyName, ValidContactName, ValidContactEmail, ValidIndustry, score));
    }
}
