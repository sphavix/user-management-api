using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoleBasedUserManagementApi.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPhoneField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "2d300243-9dff-4eb7-8231-df00eb09ecf7" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2d300243-9dff-4eb7-8231-df00eb09ecf7");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1ef6d9c5-61ec-433b-9c6b-5f114b119b66", 0, "d5ed70b0-59df-4433-80dc-3eedfa07e168", "rolebasedadmin@geeking.com", true, false, null, "ROLEBASEDADMIN@GEEKING.COM", "ROLEBASEDADMIN@GEEKING.COM", "AQAAAAIAAYagAAAAEIX+0kroaNCFkDaOuyxD01bV1CqhkRikO+j3sECdIxrYWRlN7pRYsNnbsraLNR4COg==", "0812761542", true, "9e0c0154-7a0f-453f-a228-605852574f26", false, "rolebasedadmin@geeking.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1ef6d9c5-61ec-433b-9c6b-5f114b119b66" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1ef6d9c5-61ec-433b-9c6b-5f114b119b66" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1ef6d9c5-61ec-433b-9c6b-5f114b119b66");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "2d300243-9dff-4eb7-8231-df00eb09ecf7", 0, "37ea6293-b4d3-499a-981f-4859f26340c8", "rolebasedadmin@geeking.com", true, false, null, "ROLEBASEDADMIN@GEEKING.COM", "ROLEBASEDADMIN@GEEKING.COM", "AQAAAAIAAYagAAAAEL+Vyf6tJ3xlwuiLRIX5IgRq9xblCXxDR4UwoJlZ2ticiL7ut8Q2ArkKZ96RWcG6yg==", "0812761542", true, "197d7c06-2b0d-40ed-b1f7-96b079638b5e", false, "rolebasedadmin@geeking.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "2d300243-9dff-4eb7-8231-df00eb09ecf7" });
        }
    }
}
