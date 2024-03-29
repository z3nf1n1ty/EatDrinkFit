using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatDrinkFit.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class _002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
