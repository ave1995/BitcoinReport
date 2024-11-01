using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grid.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "Time",
                table: "bitcoin_detail",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "bitcoin_detail");
        }
    }
}
