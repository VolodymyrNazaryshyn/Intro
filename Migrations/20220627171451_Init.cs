using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Intro.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassSalt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegMoment = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogMoment = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Avatar", "Email", "LogMoment", "Login", "PassHash", "PassSalt", "RealName", "RegMoment" },
                values: new object[] { new Guid("e4dc2bed-e0ef-4490-84ab-45a1f2677aa4"), "", null, null, "Admin", "", "", "Корневой администратор", new DateTime(2022, 6, 27, 20, 14, 51, 453, DateTimeKind.Local).AddTicks(6662) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
