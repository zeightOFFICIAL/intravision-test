using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "brands",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_brands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "coins",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    denomination = table.Column<int>(type: "integer", nullable: false),
                    amount_available = table.Column<int>(type: "integer", nullable: false),
                    is_blocked = table.Column<bool>(type: "boolean", nullable: false),
                    coin_type = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coins", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    amount_paid = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    change_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", nullable: false),
                    failure_reason = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "drinks",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    amount = table.Column<int>(type: "integer", nullable: false),
                    brand_id = table.Column<int>(type: "integer", nullable: false),
                    image_url = table.Column<string>(type: "varchar(255)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_drinks", x => x.id);
                    table.ForeignKey(
                        name: "FK_drinks_brands_brand_id",
                        column: x => x.brand_id,
                        principalSchema: "public",
                        principalTable: "brands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    drink_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    brand_name = table.Column<string>(type: "varchar(100)", nullable: false),
                    price_at_purchase = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    image_url_at_purchase = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalSchema: "public",
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "brands",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Coca-Cola" },
                    { 2, "Pepsi" },
                    { 3, "Nestlé" },
                    { 4, "Lifeline" },
                    { 5, "Добрый" },
                    { 6, "Lipton" },
                    { 7, "Шишкин лес" }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "coins",
                columns: new[] { "id", "amount_available", "coin_type", "denomination", "is_blocked" },
                values: new object[,]
                {
                    { 1, 25, "Regular", 1, false },
                    { 2, 10, "Regular", 2, false },
                    { 3, 30, "Regular", 5, false },
                    { 4, 20, "Regular", 10, false }
                });

            migrationBuilder.InsertData(
                schema: "public",
                table: "drinks",
                columns: new[] { "id", "amount", "brand_id", "description", "image_url", "name", "price" },
                values: new object[,]
                {
                    { 1, 5, 1, null, "colao.png", "Coca-Cola Classic 333ml", 103m },
                    { 2, 2, 1, null, "colao.png", "Coca-Cola Classic 499ml", 131m },
                    { 3, 3, 1, null, "colaz.png", "Coca-Cola Zero Sugar 333ml", 97m },
                    { 5, 3, 2, null, "pepsi.png", "Pepsi 499ml", 121m },
                    { 6, 4, 2, null, "pepsim.png", "Pepsi Max 499ml", 140m },
                    { 7, 2, 2, null, "pepsim.png", "Pepsi Max 333ml", 121m },
                    { 8, 7, 2, null, "pepsi.png", "Pepsi 333ml", 113m },
                    { 9, 0, 3, null, "nesteag.png", "Nestea Green Tea 333ml", 131m },
                    { 10, 1, 3, null, "nesteal.png", "Nestea Lemon 333ml", 121m },
                    { 11, 0, 3, null, "nesteap.png", "Nestea Peach 333ml", 124m },
                    { 12, 13, 3, null, "nestear.png", "Nestea Raspberry 333ml", 146m },
                    { 13, 3, 4, null, "lifelinew.png", "Lifeline Watermelon 499ml", 151m },
                    { 14, 2, 4, null, "lifelinel.png", "Lifeline Lichi 499ml", 140m },
                    { 15, 0, 4, null, "lifelinep.png", "Lifeline Peach 499ml", 135m },
                    { 16, 5, 5, null, "dobriyo.png", "Добрый Апельсин 499ml", 190m },
                    { 17, 4, 5, null, "dobriya.png", "Добрый Яблоко 499ml", 192m },
                    { 18, 3, 5, null, "dobriym.png", "Добрый Мультифрукт 499ml", 181m },
                    { 19, 5, 6, null, "lipton.png", "Lipton Ice Tea Lemon 499ml", 148m },
                    { 20, 9, 6, null, "liptonp.png", "Lipton Ice Tea Peach 499ml", 187m },
                    { 21, 2, 6, null, "liptong.png", "Lipton Green Tea 499ml", 201m },
                    { 22, 15, 7, null, "sl.png", "Шишкин лес Газированная 333ml", 64m },
                    { 23, 4, 7, null, "sl.png", "Шишкин лес Негазированная 333ml", 44m },
                    { 24, 4, 7, null, "sl.png", "Шишкин лес Минеральная 333ml", 32m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_drinks_brand_id",
                schema: "public",
                table: "drinks",
                column: "brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                schema: "public",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_orders_order_date",
                schema: "public",
                table: "orders",
                column: "order_date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coins",
                schema: "public");

            migrationBuilder.DropTable(
                name: "drinks",
                schema: "public");

            migrationBuilder.DropTable(
                name: "order_items",
                schema: "public");

            migrationBuilder.DropTable(
                name: "brands",
                schema: "public");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "public");
        }
    }
}
