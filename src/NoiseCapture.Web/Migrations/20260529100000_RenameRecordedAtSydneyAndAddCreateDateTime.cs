using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoiseCapture.Web.Migrations
{
    /// <inheritdoc />
    public partial class RenameRecordedAtSydneyAndAddCreateDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecordedAtSydney",
                table: "NoiseLogEntries",
                newName: "RecordedDateTime");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreateDateTime",
                table: "NoiseLogEntries",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDateTime",
                table: "NoiseLogEntries");

            migrationBuilder.RenameColumn(
                name: "RecordedDateTime",
                table: "NoiseLogEntries",
                newName: "RecordedAtSydney");
        }
    }
}
