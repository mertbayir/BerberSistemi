using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace odev.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Barbers_BarberId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_UserID",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_BarberId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_UserID",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BarberId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Appointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarberId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BarberId",
                table: "Appointments",
                column: "BarberId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserID",
                table: "Appointments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Barbers_BarberId",
                table: "Appointments",
                column: "BarberId",
                principalTable: "Barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_UserID",
                table: "Appointments",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
