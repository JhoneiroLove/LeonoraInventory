using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeonoraInventory.Migrations
{
    /// <inheritdoc />
    public partial class ProveedorProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Proveedor",
                table: "Productos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Proveedor",
                table: "Productos");
        }
    }
}
