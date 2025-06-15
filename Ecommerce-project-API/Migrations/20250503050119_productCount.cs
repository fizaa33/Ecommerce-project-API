using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce_project_API.Migrations
{
    /// <inheritdoc />
    public partial class productCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SearchCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoldQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SoldQuantity",
                table: "Products");
        }
    }
}
