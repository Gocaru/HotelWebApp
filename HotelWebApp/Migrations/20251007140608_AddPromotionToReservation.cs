using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Reservations",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Reservations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromotionId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PromotionId",
                table: "Reservations",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Promotions_PromotionId",
                table: "Reservations",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Promotions_PromotionId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_PromotionId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PromotionId",
                table: "Reservations");
        }
    }
}
