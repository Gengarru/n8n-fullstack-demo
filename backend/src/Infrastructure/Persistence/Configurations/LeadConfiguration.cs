using LeadEnrichment.Domain.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeadEnrichment.Infrastructure.Persistence.Configurations;

public sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("leads");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CompanyName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ContactName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ContactEmail)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Industry)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Score)
            .IsRequired();

        // No fluent config needed (nullable, no length/required constraint) —
        // listed explicitly so every Lead property is accounted for here.
        builder.Property(x => x.EnrichedAt);

        builder.HasIndex(x => x.CompanyName);
    }
}
