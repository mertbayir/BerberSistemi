using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev.Migrations
{
    /// <inheritdoc />
    public partial class SureUcretEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Appointments");
        }
    }
}
