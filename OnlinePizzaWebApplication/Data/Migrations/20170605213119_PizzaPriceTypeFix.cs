using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;


namespace OnlinePizzaWebApplication.Migrations
{
    public partial class PizzaPriceTypeFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Pizzas",
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Barcode",
                table: "Pizzas",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
