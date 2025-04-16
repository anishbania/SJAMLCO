using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Migrations
{
    /// <inheritdoc />
    public partial class courierupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Qty",
                table: "LogisticItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModeOfCourier",
                table: "LogisticDispatches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModeOfCourier",
                table: "LogisticDispatches");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "LogisticItems",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
