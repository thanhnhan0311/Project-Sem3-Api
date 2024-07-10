using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class refund1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_PaymentId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Refunds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_PaymentId",
                table: "Refunds",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
