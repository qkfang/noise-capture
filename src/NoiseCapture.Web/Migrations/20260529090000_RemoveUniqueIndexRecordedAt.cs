using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoiseCapture.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueIndexRecordedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NoiseLogEntries_RecordedAtSydney",
                table: "NoiseLogEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NoiseLogEntries_RecordedAtSydney",
                table: "NoiseLogEntries",
                column: "RecordedAtSydney",
                unique: true);
        }
    }
}
