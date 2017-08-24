using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace mysqltest.Migrations
{
    public partial class AgeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Person",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Person");
        }
    }
}
