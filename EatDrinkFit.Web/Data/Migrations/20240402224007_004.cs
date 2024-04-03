using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatDrinkFit.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class _004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DashboardMicroChartEnteries",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cholesterol = table.Column<long>(type: "bigint", nullable: false),
                    Sodium = table.Column<long>(type: "bigint", nullable: false),
                    Fiber = table.Column<long>(type: "bigint", nullable: false),
                    Sugar = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardMicroChartEnteries", x => new { x.UserId, x.LogDate });
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardMicroChartEnteries_Id",
                table: "DashboardMicroChartEnteries",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardMicroChartEnteries");
        }
    }
}
