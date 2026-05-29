using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoiseCapture.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NoiseLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecordedAtSydney = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Intensity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Loudness = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Tone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ContinuedFromLast = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiseLogEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoiseLogEntryLocations",
                columns: table => new
                {
                    NoiseLogEntryId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiseLogEntryLocations", x => new { x.NoiseLogEntryId, x.SortOrder });
                    table.ForeignKey(
                        name: "FK_NoiseLogEntryLocations_NoiseLogEntries_NoiseLogEntryId",
                        column: x => x.NoiseLogEntryId,
                        principalTable: "NoiseLogEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoiseLogEntryNoiseSources",
                columns: table => new
                {
                    NoiseLogEntryId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoiseLogEntryNoiseSources", x => new { x.NoiseLogEntryId, x.SortOrder });
                    table.ForeignKey(
                        name: "FK_NoiseLogEntryNoiseSources_NoiseLogEntries_NoiseLogEntryId",
                        column: x => x.NoiseLogEntryId,
                        principalTable: "NoiseLogEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NoiseLogEntries_RecordedAtSydney",
                table: "NoiseLogEntries",
                column: "RecordedAtSydney",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoiseLogEntryLocations");

            migrationBuilder.DropTable(
                name: "NoiseLogEntryNoiseSources");

            migrationBuilder.DropTable(
                name: "NoiseLogEntries");
        }
    }
}
