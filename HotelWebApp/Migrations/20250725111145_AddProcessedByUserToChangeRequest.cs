using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessedByUserToChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProcessedByUserId",
                table: "ChangeRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessedByUserId",
                table: "ChangeRequests");
        }
    }
}
