using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBarberService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarberService_Barbers_BarberId",
                table: "BarberService");

            migrationBuilder.DropForeignKey(
                name: "FK_BarberService_Services_ServiceId",
                table: "BarberService");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BarberService",
                table: "BarberService");

            migrationBuilder.RenameTable(
                name: "BarberService",
                newName: "BarberServices");

            migrationBuilder.RenameIndex(
                name: "IX_BarberService_BarberId",
                table: "BarberServices",
                newName: "IX_BarberServices_BarberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BarberServices",
                table: "BarberServices",
                columns: new[] { "ServiceId", "BarberId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BarberServices_Barbers_BarberId",
                table: "BarberServices",
                column: "BarberId",
                principalTable: "Barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BarberServices_Services_ServiceId",
                table: "BarberServices",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarberServices_Barbers_BarberId",
                table: "BarberServices");

            migrationBuilder.DropForeignKey(
                name: "FK_BarberServices_Services_ServiceId",
                table: "BarberServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BarberServices",
                table: "BarberServices");

            migrationBuilder.RenameTable(
                name: "BarberServices",
                newName: "BarberService");

            migrationBuilder.RenameIndex(
                name: "IX_BarberServices_BarberId",
                table: "BarberService",
                newName: "IX_BarberService_BarberId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BarberService",
                table: "BarberService",
                columns: new[] { "ServiceId", "BarberId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BarberService_Barbers_BarberId",
                table: "BarberService",
                column: "BarberId",
                principalTable: "Barbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BarberService_Services_ServiceId",
                table: "BarberService",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
