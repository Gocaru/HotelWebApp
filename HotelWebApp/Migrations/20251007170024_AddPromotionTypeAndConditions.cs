using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionTypeAndConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinimumNights",
                table: "Promotions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Promotions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumNights",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Promotions");
        }
    }
}
