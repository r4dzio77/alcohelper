using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlcoHelper.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToAlcoholFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApproves",
                table: "Alcohols",
                newName: "IsApproved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "Alcohols",
                newName: "IsApproves");
        }
    }
}
