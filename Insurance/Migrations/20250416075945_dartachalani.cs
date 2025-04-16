using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Migrations
{
    /// <inheritdoc />
    public partial class dartachalani : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefNumber",
                table: "LogisticDispatches");

            migrationBuilder.AddColumn<string>(
                name: "ChalaniNo",
                table: "LogisticDispatches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DartaNo",
                table: "LogisticDispatches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChalaniNo",
                table: "LogisticDispatches");

            migrationBuilder.DropColumn(
                name: "DartaNo",
                table: "LogisticDispatches");

            migrationBuilder.AddColumn<string>(
                name: "RefNumber",
                table: "LogisticDispatches",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);
        }
    }
}
