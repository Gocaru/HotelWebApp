using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelWebApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameGuestIdToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GuestId",
                table: "Guests",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Guests",
                newName: "GuestId");
        }
    }
}
