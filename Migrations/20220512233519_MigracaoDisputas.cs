using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rpg_Api.Migrations
{
    public partial class MigracaoDisputas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Perfil",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "Jogador",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Jogador");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Derrotas",
                table: "Personagens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Disputas",
                table: "Personagens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Vitorias",
                table: "Personagens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Disputas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataDisputa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtacanteId = table.Column<int>(type: "int", nullable: false),
                    OponenteId = table.Column<int>(type: "int", nullable: false),
                    Narracao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disputas", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 45, 173, 16, 96, 237, 19, 57, 223, 44, 205, 60, 116, 117, 45, 39, 88, 47, 93, 81, 9, 195, 69, 193, 105, 153, 206, 205, 39, 84, 184, 13, 238, 51, 247, 22, 31, 249, 63, 95, 89, 177, 185, 82, 205, 29, 193, 227, 141, 148, 157, 228, 2, 16, 179, 205, 197, 47, 34, 226, 167, 214, 85, 211, 64 }, new byte[] { 30, 205, 245, 94, 123, 195, 75, 45, 201, 175, 170, 192, 193, 171, 233, 124, 113, 252, 119, 111, 58, 173, 67, 60, 124, 43, 123, 14, 62, 111, 40, 241, 229, 250, 157, 72, 8, 182, 218, 208, 57, 213, 29, 150, 51, 180, 245, 217, 190, 102, 43, 210, 86, 227, 87, 168, 128, 196, 28, 1, 162, 159, 75, 2, 153, 210, 162, 183, 111, 226, 51, 126, 252, 48, 26, 79, 211, 225, 76, 126, 113, 142, 230, 45, 206, 104, 145, 206, 249, 37, 136, 162, 146, 40, 248, 236, 85, 79, 130, 179, 113, 76, 29, 189, 172, 58, 149, 130, 236, 169, 39, 212, 242, 137, 166, 74, 183, 29, 9, 151, 241, 75, 255, 222, 192, 140, 167, 65 } });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disputas");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Derrotas",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "Disputas",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "Vitorias",
                table: "Personagens");

            migrationBuilder.AlterColumn<string>(
                name: "Perfil",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Jogador",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "Jogador");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { new byte[] { 30, 91, 241, 118, 196, 188, 149, 198, 54, 119, 238, 108, 44, 39, 233, 179, 219, 37, 12, 96, 98, 243, 219, 5, 232, 55, 131, 149, 115, 160, 142, 149, 211, 102, 65, 95, 205, 59, 29, 60, 89, 31, 254, 224, 183, 141, 187, 197, 62, 47, 159, 136, 17, 200, 176, 96, 10, 43, 212, 37, 166, 213, 124, 97 }, new byte[] { 161, 96, 101, 229, 32, 208, 205, 4, 161, 26, 244, 43, 46, 159, 3, 203, 58, 211, 32, 22, 91, 100, 251, 12, 218, 72, 100, 31, 11, 66, 221, 221, 61, 69, 138, 197, 97, 82, 73, 245, 202, 245, 67, 28, 34, 90, 205, 51, 206, 192, 106, 17, 42, 116, 235, 51, 206, 152, 240, 29, 146, 118, 150, 47, 22, 2, 211, 116, 179, 25, 125, 221, 126, 122, 203, 243, 106, 90, 1, 119, 47, 28, 126, 72, 32, 174, 178, 120, 170, 78, 25, 49, 196, 220, 99, 6, 239, 207, 169, 30, 126, 187, 62, 99, 38, 136, 153, 103, 54, 161, 4, 167, 34, 23, 246, 12, 36, 234, 169, 229, 106, 76, 9, 221, 172, 171, 128, 242 } });
        }
    }
}
