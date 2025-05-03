using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlcoHelper.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToAlcohol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductUrl",
                table: "AlcoholStores",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproves",
                table: "Alcohols",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproves",
                table: "Alcohols");

            migrationBuilder.UpdateData(
                table: "AlcoholStores",
                keyColumn: "ProductUrl",
                keyValue: null,
                column: "ProductUrl",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ProductUrl",
                table: "AlcoholStores",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
