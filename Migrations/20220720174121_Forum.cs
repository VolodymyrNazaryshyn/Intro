using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intro.Migrations
{
    public partial class Forum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e4dc2bed-e0ef-4490-84ab-45a1f2677aa4"));

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PictureFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("826ee5ab-e889-4743-9be6-51a681c40dd4"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 7, 20, 20, 41, 21, 256, DateTimeKind.Local).AddTicks(4684) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("826ee5ab-e889-4743-9be6-51a681c40dd4"));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("e4dc2bed-e0ef-4490-84ab-45a1f2677aa4"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 6, 27, 20, 14, 51, 453, DateTimeKind.Local).AddTicks(6662) });
        }
    }
}
