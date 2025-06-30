using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLendingBackUp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class desired_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DesiredRole",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DesiredRole",
                table: "AspNetUsers");
        }
    }
}
