using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intro.Migrations
{
    public partial class ArticleDeleteMoment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1f0263e-3425-436b-a846-61403d801153"));

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Articles",
                newName: "CreatedMoment");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteMoment",
                table: "Articles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("2002755e-4a83-4470-88bd-7102c6300f26"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 26, 20, 21, 43, 107, DateTimeKind.Local).AddTicks(2015) });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ReplyId",
                table: "Articles",
                column: "ReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Articles_ReplyId",
                table: "Articles",
                column: "ReplyId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Articles_ReplyId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ReplyId",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2002755e-4a83-4470-88bd-7102c6300f26"));

            migrationBuilder.DropColumn(
                name: "DeleteMoment",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "CreatedMoment",
                table: "Articles",
                newName: "CreatedDate");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("a1f0263e-3425-436b-a846-61403d801153"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 22, 18, 31, 33, 401, DateTimeKind.Local).AddTicks(6162) });
        }
    }
}
