using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCancel",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancel",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsCancel",
                table: "Orders");

            migrationBuilder.AddColumn<bool>(
                name: "IsCancel",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
