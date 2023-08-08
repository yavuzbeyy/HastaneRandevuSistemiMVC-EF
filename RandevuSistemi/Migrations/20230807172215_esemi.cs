using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class esemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "DoktorId",
                table: "Randevular");

            migrationBuilder.RenameColumn(
                name: "RandevuSaati",
                table: "Randevular",
                newName: "randevuSaatiId");

            migrationBuilder.CreateTable(
                name: "CalismaSaatleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    CalismaZamani = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalismaSaatleri", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Randevular_randevuSaatiId",
                table: "Randevular",
                column: "randevuSaatiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Randevular_CalismaSaatleri_randevuSaatiId",
                table: "Randevular",
                column: "randevuSaatiId",
                principalTable: "CalismaSaatleri",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_CalismaSaatleri_randevuSaatiId",
                table: "Randevular");

            migrationBuilder.DropTable(
                name: "CalismaSaatleri");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_randevuSaatiId",
                table: "Randevular");

            migrationBuilder.RenameColumn(
                name: "randevuSaatiId",
                table: "Randevular",
                newName: "RandevuSaati");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Randevular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DoktorId",
                table: "Randevular",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
