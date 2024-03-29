using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatDrinkFit.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class _003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DashboardCalorieChartEntry",
                table: "DashboardCalorieChartEntry");

            migrationBuilder.RenameTable(
                name: "DashboardCalorieChartEntry",
                newName: "DashboardCalorieChartEnteries");

            migrationBuilder.RenameIndex(
                name: "IX_DashboardCalorieChartEntry_Id",
                table: "DashboardCalorieChartEnteries",
                newName: "IX_DashboardCalorieChartEnteries_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DashboardCalorieChartEnteries",
                table: "DashboardCalorieChartEnteries",
                columns: new[] { "UserId", "LogDate" });

            migrationBuilder.CreateTable(
                name: "DashboardMacroChartEnteries",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fat = table.Column<long>(type: "bigint", nullable: false),
                    Carb = table.Column<long>(type: "bigint", nullable: false),
                    Protein = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardMacroChartEnteries", x => new { x.UserId, x.LogDate });
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardMacroChartEnteries_Id",
                table: "DashboardMacroChartEnteries",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardMacroChartEnteries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DashboardCalorieChartEnteries",
                table: "DashboardCalorieChartEnteries");

            migrationBuilder.RenameTable(
                name: "DashboardCalorieChartEnteries",
                newName: "DashboardCalorieChartEntry");

            migrationBuilder.RenameIndex(
                name: "IX_DashboardCalorieChartEnteries_Id",
                table: "DashboardCalorieChartEntry",
                newName: "IX_DashboardCalorieChartEntry_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DashboardCalorieChartEntry",
                table: "DashboardCalorieChartEntry",
                columns: new[] { "UserId", "LogDate" });
        }
    }
}
