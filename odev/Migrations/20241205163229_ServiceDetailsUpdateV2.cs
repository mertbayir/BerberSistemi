using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev.Migrations
{
    /// <inheritdoc />
    public partial class ServiceDetailsUpdateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Service",
                table: "ServicePriceDurations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Service",
                table: "ServicePriceDurations");
        }
    }
}
