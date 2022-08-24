using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intro.Migrations
{
    public partial class TopicDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cc2d3840-9fda-4069-b6b3-bf6be0730f3b"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastArticleMoment",
                table: "Topics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("a1f0263e-3425-436b-a846-61403d801153"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 22, 18, 31, 33, 401, DateTimeKind.Local).AddTicks(6162) });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Users_AuthorId",
                table: "Topics",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Users_AuthorId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_AuthorId",
                table: "Topics");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1f0263e-3425-436b-a846-61403d801153"));

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "LastArticleMoment",
                table: "Topics");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("cc2d3840-9fda-4069-b6b3-bf6be0730f3b"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 8, 17, 20, 43, 12, 754, DateTimeKind.Local).AddTicks(7698) });
        }
    }
}
