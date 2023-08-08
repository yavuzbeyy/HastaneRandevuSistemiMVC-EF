using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RandevuSistemi.Migrations
{
    /// <inheritdoc />
    public partial class solo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Randevular_CalismaSaatleri_randevuSaatiId",
                table: "Randevular");

            migrationBuilder.DropIndex(
                name: "IX_Randevular_randevuSaatiId",
                table: "Randevular");

            migrationBuilder.RenameColumn(
                name: "randevuSaatiId",
                table: "Randevular",
                newName: "DoctorId");

            migrationBuilder.AddColumn<string>(
                name: "DoctorAdi",
                table: "Randevular",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RandevuSaati",
                table: "Randevular",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DoctorAdi",
                table: "CalismaSaatleri",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorAdi",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "RandevuSaati",
                table: "Randevular");

            migrationBuilder.DropColumn(
                name: "DoctorAdi",
                table: "CalismaSaatleri");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Randevular",
                newName: "randevuSaatiId");

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
    }
}
