using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommUnityHub.Migrations
{
    /// <inheritdoc />
    public partial class UpdateResourceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City",
                table: "Resources",
                newName: "Region");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Region",
                table: "Resources",
                newName: "City");
        }
    }
}
