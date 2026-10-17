using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VehicleCategoryManager.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManufacturerName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.ManufacturerId);
                });

            migrationBuilder.CreateTable(
                name: "VehicleWeightClasses",
                columns: table => new
                {
                    VehicleWeightClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleWeightClassName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleWeightClassIconPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinWeightKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    MaxWeightKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleWeightClasses", x => x.VehicleWeightClassId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeightKg = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    ManufacturerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_Manufacturers_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturers",
                        principalColumn: "ManufacturerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Manufacturers",
                columns: new[] { "ManufacturerId", "ManufacturerName" },
                values: new object[,]
                {
                    { 1, "Mazda" },
                    { 2, "Mercedes" },
                    { 3, "Honda" },
                    { 4, "Ferrari" },
                    { 5, "Toyota" }
                });

            migrationBuilder.InsertData(
                table: "VehicleWeightClasses",
                columns: new[] { "VehicleWeightClassId", "MaxWeightKg", "MinWeightKg", "VehicleWeightClassIconPath", "VehicleWeightClassName" },
                values: new object[,]
                {
                    { 1, 500m, 0m, "icons/light-weight.svg", "Light" },
                    { 2, 2500m, 500m, "icons/med-weight.svg", "Medium" },
                    { 3, null, 2500m, "icons/heavy-weight.svg", "Heavy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ManufacturerId",
                table: "Vehicles",
                column: "ManufacturerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleWeightClasses");

            migrationBuilder.DropTable(
                name: "Manufacturers");
        }
    }
}
