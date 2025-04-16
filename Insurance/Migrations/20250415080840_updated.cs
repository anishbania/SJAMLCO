using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Insurance.Migrations
{
    /// <inheritdoc />
    public partial class updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourierSupportingFiles");

            migrationBuilder.AddColumn<string>(
                name: "SupportingFilePath",
                table: "LogisticDispatches",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportingFilePath",
                table: "LogisticDispatches");

            migrationBuilder.CreateTable(
                name: "CourierSupportingFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchId = table.Column<int>(type: "int", nullable: false),
                    SupportingFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupportingFilePath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierSupportingFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourierSupportingFiles_LogisticDispatches_DispatchId",
                        column: x => x.DispatchId,
                        principalTable: "LogisticDispatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourierSupportingFiles_DispatchId",
                table: "CourierSupportingFiles",
                column: "DispatchId");
        }
    }
}
