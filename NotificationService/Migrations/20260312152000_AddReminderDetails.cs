using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NotificationService.Persistence;

#nullable disable

namespace NotificationService.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    [Migration("20260312152000_AddReminderDetails")]
    public partial class AddReminderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                    name: "CustomerName",
                    table: "Reminders",
                    type: "longtext",
                    nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                    name: "ServiceName",
                    table: "Reminders",
                    type: "longtext",
                    nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "AppointmentTime",
                table: "Reminders",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "AppointmentTime",
                table: "Reminders");
        }
    }
}
