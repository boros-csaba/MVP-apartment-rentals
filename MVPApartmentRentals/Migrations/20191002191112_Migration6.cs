using Microsoft.EntityFrameworkCore.Migrations;

namespace MVPApartmentRentals.Migrations
{
    public partial class Migration6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Used",
                table: "RefreshTokens");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "RefreshTokens",
                nullable: false,
                defaultValue: false);
        }
    }
}
