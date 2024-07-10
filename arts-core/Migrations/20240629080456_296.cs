using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class _296 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_OrderId",
                table: "Refunds");

            migrationBuilder.AddColumn<float>(
                name: "AmountRefund",
                table: "Refunds",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Refunds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Refunds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "OriginalOrderId",
                table: "Exchanges",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NewOrderId",
                table: "Exchanges",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_OrderId",
                table: "Refunds",
                column: "OrderId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges",
                column: "NewOrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_OrderId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "AmountRefund",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Refunds");

            migrationBuilder.AlterColumn<int>(
                name: "OriginalOrderId",
                table: "Exchanges",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NewOrderId",
                table: "Exchanges",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_OrderId",
                table: "Refunds",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exchanges_Orders_NewOrderId",
                table: "Exchanges",
                column: "NewOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Payments_PaymentId",
                table: "Refunds",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }
    }
}
