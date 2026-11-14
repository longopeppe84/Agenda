using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Agenda2.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPizzasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pizzas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ingredients = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImageUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pizzas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Pizzas",
                columns: new[] { "Id", "ImageUrl", "Ingredients", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "🍕", "Pomodoro, Mozzarella, Basilico", false, "Margherita", 5.50m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "🌶️", "Pomodoro, Mozzarella, Salame Piccante", false, "Diavola", 7.00m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "🍄", "Pomodoro, Mozzarella, Prosciutto, Funghi, Carciofi", false, "Capricciosa", 8.50m },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "🧀", "Mozzarella, Gorgonzola, Fontina, Parmigiano", false, "Quattro Formaggi", 8.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pizzas");
        }
    }
}
