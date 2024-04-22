using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatDrinkFit.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class _005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DashboardPercentCalChartEntry",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PercentOther = table.Column<long>(type: "bigint", nullable: false),
                    PercentFat = table.Column<long>(type: "bigint", nullable: false),
                    PercentCarb = table.Column<long>(type: "bigint", nullable: false),
                    PercentProtein = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardPercentCalChartEntry", x => new { x.UserId, x.LogDate });
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashboardPercentCalChartEntry_Id",
                table: "DashboardPercentCalChartEntry",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashboardPercentCalChartEntry");
        }
    }
}
