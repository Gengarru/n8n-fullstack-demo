using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LeadEnrichment.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    ContactName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Industry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EnrichedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_leads", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "leads",
                columns: new[] { "Id", "CompanyName", "ContactEmail", "ContactName", "EnrichedAt", "Industry", "Score", "Status" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111001"), "Nordwind Logistik GmbH", "p.lindgren@nordwind-logistik.example", "Petra Lindgren", new DateTimeOffset(new DateTime(2026, 6, 30, 9, 12, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Logistik", 87, "Qualified" },
                    { new Guid("11111111-1111-1111-1111-111111111002"), "Blaufeld Systemtechnik AG", "m.vogt@blaufeld-systemtechnik.example", "Markus Vogt", new DateTimeOffset(new DateTime(2026, 7, 2, 14, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Maschinenbau", 64, "Enriched" },
                    { new Guid("11111111-1111-1111-1111-111111111003"), "Rheinallee Consulting Partners", "s.hoff@rheinallee-consulting.example", "Sabine Hoff", new DateTimeOffset(new DateTime(2026, 6, 28, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Beratung", 52, "Contacted" },
                    { new Guid("11111111-1111-1111-1111-111111111004"), "Kupferbach Elektronik KG", "j.wetter@kupferbach-elektronik.example", "Jonas Wetter", null, "Elektronik", 12, "New" },
                    { new Guid("11111111-1111-1111-1111-111111111005"), "Silberpfad Media Solutions", "a.brummer@silberpfad-media.example", "Anke Brummer", new DateTimeOffset(new DateTime(2026, 7, 5, 8, 5, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Medien", 71, "Enriched" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_leads_CompanyName",
                table: "leads",
                column: "CompanyName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "leads");
        }
    }
}
