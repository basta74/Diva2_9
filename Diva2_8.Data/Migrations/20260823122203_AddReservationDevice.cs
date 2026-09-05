using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diva2_8.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "device",
                table: "spinuser",
                type: "tinyint unsigned",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device",
                table: "spinuser");
        }
    }
}
