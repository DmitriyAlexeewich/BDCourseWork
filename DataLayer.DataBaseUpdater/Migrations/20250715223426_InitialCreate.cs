using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataLayer.DataBaseUpdater.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "priceRule",
                columns: table => new
                {
                    storeClass = table.Column<string>(type: "text", nullable: false),
                    productGrade = table.Column<string>(type: "text", nullable: false),
                    startDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    endDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_priceRule", x => new { x.storeClass, x.productGrade, x.startDate });
                });

            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    grade = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_product", x => new { x.name, x.grade });
                });

            migrationBuilder.CreateTable(
                name: "warehouse",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouse", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "store",
                columns: table => new
                {
                    number = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    @class = table.Column<string>(name: "class", type: "text", nullable: false),
                    warehouseName = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_store", x => x.number);
                    table.ForeignKey(
                        name: "FK_store_warehouse_warehouseName",
                        column: x => x.warehouseName,
                        principalTable: "warehouse",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "warehouseProduct",
                columns: table => new
                {
                    productName = table.Column<string>(type: "text", nullable: false),
                    productGrade = table.Column<string>(type: "text", nullable: false),
                    warehouseName = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouseProduct", x => new { x.warehouseName, x.productName, x.productGrade });
                    table.ForeignKey(
                        name: "FK_warehouseProduct_product_productName_productGrade",
                        columns: x => new { x.productName, x.productGrade },
                        principalTable: "product",
                        principalColumns: new[] { "name", "grade" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_warehouseProduct_warehouse_warehouseName",
                        column: x => x.warehouseName,
                        principalTable: "warehouse",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "department",
                columns: table => new
                {
                    number = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    head = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_department", x => new { x.number, x.name });
                    table.ForeignKey(
                        name: "FK_department_store_number",
                        column: x => x.number,
                        principalTable: "store",
                        principalColumn: "number",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "departmentProduct",
                columns: table => new
                {
                    productName = table.Column<string>(type: "text", nullable: false),
                    productGrade = table.Column<string>(type: "text", nullable: false),
                    storeNumber = table.Column<int>(type: "integer", nullable: false),
                    departmentName = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "timezone('utc', now())"),
                    is_removed = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_departmentProduct", x => new { x.storeNumber, x.departmentName, x.productName, x.productGrade });
                    table.ForeignKey(
                        name: "FK_departmentProduct_department_storeNumber_departmentName",
                        columns: x => new { x.storeNumber, x.departmentName },
                        principalTable: "department",
                        principalColumns: new[] { "number", "name" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_departmentProduct_product_productName_productGrade",
                        columns: x => new { x.productName, x.productGrade },
                        principalTable: "product",
                        principalColumns: new[] { "name", "grade" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_department_is_removed",
                table: "department",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_departmentProduct_is_removed",
                table: "departmentProduct",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_departmentProduct_productName_productGrade",
                table: "departmentProduct",
                columns: new[] { "productName", "productGrade" });

            migrationBuilder.CreateIndex(
                name: "IX_priceRule_is_removed",
                table: "priceRule",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_product_is_removed",
                table: "product",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_store_is_removed",
                table: "store",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_store_warehouseName",
                table: "store",
                column: "warehouseName");

            migrationBuilder.CreateIndex(
                name: "IX_warehouse_is_removed",
                table: "warehouse",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_warehouseProduct_is_removed",
                table: "warehouseProduct",
                column: "is_removed");

            migrationBuilder.CreateIndex(
                name: "IX_warehouseProduct_productName_productGrade",
                table: "warehouseProduct",
                columns: new[] { "productName", "productGrade" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "departmentProduct");

            migrationBuilder.DropTable(
                name: "priceRule");

            migrationBuilder.DropTable(
                name: "warehouseProduct");

            migrationBuilder.DropTable(
                name: "department");

            migrationBuilder.DropTable(
                name: "product");

            migrationBuilder.DropTable(
                name: "store");

            migrationBuilder.DropTable(
                name: "warehouse");
        }
    }
}
