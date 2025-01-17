using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoleBasedUserManagementApi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedTokenIfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1ef6d9c5-61ec-433b-9c6b-5f114b119b66" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1ef6d9c5-61ec-433b-9c6b-5f114b119b66");

            migrationBuilder.CreateTable(
                name: "TokenInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenInfo", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4e282a9f-c1ea-4fca-8ce0-0bfb4786277b", 0, "72da6578-5f9f-465a-befa-e245cd168b26", "rolebasedadmin@geeking.com", true, false, null, "ROLEBASEDADMIN@GEEKING.COM", "ROLEBASEDADMIN@GEEKING.COM", "AQAAAAIAAYagAAAAEN9o/8/a+zTRuThGJRXZOXx8jPFmubtfnYGJL+ivoRJoNEJKB6PjNDalm3b8Ysgd6Q==", "0812761542", true, "5fe4765f-4d7a-4621-a6a4-e38e1503ce47", false, "rolebasedadmin@geeking.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "4e282a9f-c1ea-4fca-8ce0-0bfb4786277b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenInfo");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "4e282a9f-c1ea-4fca-8ce0-0bfb4786277b" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e282a9f-c1ea-4fca-8ce0-0bfb4786277b");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1ef6d9c5-61ec-433b-9c6b-5f114b119b66", 0, "d5ed70b0-59df-4433-80dc-3eedfa07e168", "rolebasedadmin@geeking.com", true, false, null, "ROLEBASEDADMIN@GEEKING.COM", "ROLEBASEDADMIN@GEEKING.COM", "AQAAAAIAAYagAAAAEIX+0kroaNCFkDaOuyxD01bV1CqhkRikO+j3sECdIxrYWRlN7pRYsNnbsraLNR4COg==", "0812761542", true, "9e0c0154-7a0f-453f-a228-605852574f26", false, "rolebasedadmin@geeking.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1ef6d9c5-61ec-433b-9c6b-5f114b119b66" });
        }
    }
}
