using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EatDrinkFit.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial_MacroLog_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MacroLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Fat = table.Column<int>(type: "int", nullable: false),
                    Cholesterol = table.Column<int>(type: "int", nullable: false),
                    TotalCarb = table.Column<int>(type: "int", nullable: false),
                    Fiber = table.Column<int>(type: "int", nullable: false),
                    Sugar = table.Column<int>(type: "int", nullable: false),
                    Protein = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MacroLogs");
        }
    }
}
