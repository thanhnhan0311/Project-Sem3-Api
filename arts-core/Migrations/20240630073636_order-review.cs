using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class orderreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ReviewId",
                table: "Orders",
                column: "ReviewId",
                unique: true,
                filter: "[ReviewId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Reviews_ReviewId",
                table: "Orders",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Reviews_ReviewId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ReviewId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Orders");
        }
    }
}
