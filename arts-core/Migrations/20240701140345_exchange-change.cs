using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class exchangechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Orders_OriginalOrderId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_NewOrderId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_OriginalOrderId",
                table: "Exchanges");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_NewOrderId",
                table: "Exchanges",
                column: "NewOrderId",
                unique: true,
                filter: "[NewOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_OriginalOrderId",
                table: "Exchanges",
                column: "OriginalOrderId",
                unique: true,
                filter: "[OriginalOrderId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges",
                column: "NewOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Orders_OriginalOrderId",
                table: "Exchanges",
                column: "OriginalOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Orders_OriginalOrderId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_NewOrderId",
                table: "Exchanges");

            migrationBuilder.DropIndex(
                name: "IX_Exchanges_OriginalOrderId",
                table: "Exchanges");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_NewOrderId",
                table: "Exchanges",
                column: "NewOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Exchanges_OriginalOrderId",
                table: "Exchanges",
                column: "OriginalOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges",
                column: "NewOrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Orders_OriginalOrderId",
                table: "Exchanges",
                column: "OriginalOrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
