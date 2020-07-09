using Microsoft.EntityFrameworkCore.Migrations;

namespace ApostelCargo.Data.Migrations
{
    public partial class AddModifiedPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MyProperty",
                table: "Post");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyProperty",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
