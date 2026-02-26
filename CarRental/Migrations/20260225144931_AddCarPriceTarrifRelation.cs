using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Migrations
{
    /// <inheritdoc />
    public partial class AddCarPriceTarrifRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerDay",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "PriceTariffId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_PriceTariffId",
                table: "Cars",
                column: "PriceTariffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_PriceTariffs_PriceTariffId",
                table: "Cars",
                column: "PriceTariffId",
                principalTable: "PriceTariffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_PriceTariffs_PriceTariffId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_PriceTariffId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "PriceTariffId",
                table: "Cars");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerDay",
                table: "Cars",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
