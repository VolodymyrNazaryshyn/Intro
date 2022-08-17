using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intro.Migrations
{
    public partial class Relations1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("826ee5ab-e889-4743-9be6-51a681c40dd4"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("cc2d3840-9fda-4069-b6b3-bf6be0730f3b"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 17, 20, 43, 12, 754, DateTimeKind.Local).AddTicks(7698) });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_TopicId",
                table: "Articles",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Topics_TopicId",
                table: "Articles",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Topics_TopicId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Users_AuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_TopicId",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cc2d3840-9fda-4069-b6b3-bf6be0730f3b"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("826ee5ab-e889-4743-9be6-51a681c40dd4"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 7, 20, 20, 41, 21, 256, DateTimeKind.Local).AddTicks(4684) });
        }
    }
}
