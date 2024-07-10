using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace arts_core.Migrations
{
    /// <inheritdoc />
    public partial class storeimaegs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StoreImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundId = table.Column<int>(type: "int", nullable: true),
                    ExchangeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreImages_Exchanges_ExchangeId",
                        column: x => x.ExchangeId,
                        principalTable: "Exchanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreImages_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreImages_ExchangeId",
                table: "StoreImages",
                column: "ExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreImages_RefundId",
                table: "StoreImages",
                column: "RefundId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreImages");
        }
    }
}
