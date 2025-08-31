using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Evacuation.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddLogAndEditPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "EvacuationPlans");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "EvacuationPlans");

            migrationBuilder.CreateTable(
                name: "EvacuationLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    EstimatedArrivalMinutes = table.Column<int>(type: "integer", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "integer", nullable: false),
                    EvacuatedPeople = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvacuationLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_EvacuationLogs_EvacuationZones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "EvacuationZones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvacuationLogs_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvacuationLogs_VehicleId",
                table: "EvacuationLogs",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_EvacuationLogs_ZoneId",
                table: "EvacuationLogs",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvacuationLogs");

            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "EvacuationPlans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "EvacuationPlans",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
