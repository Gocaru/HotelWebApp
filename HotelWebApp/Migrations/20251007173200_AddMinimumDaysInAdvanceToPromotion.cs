using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMinimumDaysInAdvanceToPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinimumDaysInAdvance",
                table: "Promotions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinimumDaysInAdvance",
                table: "Promotions");
        }
    }
}
