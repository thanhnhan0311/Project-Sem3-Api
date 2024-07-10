using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Types_PaymentStatusTypeId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PaymentStatusTypeId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentStatusTypeId",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentStatusTypeId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentStatusTypeId",
                table: "Payments",
                column: "PaymentStatusTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Types_PaymentStatusTypeId",
                table: "Payments",
                column: "PaymentStatusTypeId",
                principalTable: "Types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
