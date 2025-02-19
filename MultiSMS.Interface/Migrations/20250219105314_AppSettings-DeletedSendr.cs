using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class AppSettingsDeletedSendr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "ApiSettings");

            migrationBuilder.InsertData(
                table: "ApiSettings",
                columns: new[] { "ApiSettingsId", "ApiActive", "ApiName", "FastChannel", "TestMode" },
                values: new object[] { 3, false, "mProfi", true, false });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "f847a5fe-be4a-4649-8ef1-59a005916b03");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "d5d0f114-1538-4833-a33d-098c3e501559");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "e958ab01-96b2-493b-99e0-31f5c6109e05");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3faddcc9-35e1-4118-8b1b-4cfe2108656c", "AQAAAAIAAYagAAAAEK07nCfuOYdPSeU4PKSkMfrmldJwXjPwqaUwrurqiVTqZ289RLkGWbyjziwN7Xp4DA==", "NT55W4OPHEPKX7FMAP7SJUBFNJDZFHYS" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "Email", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c0e735de-3290-4692-b780-fc8f0ac89602", "admin@gmail.com", "AQAAAAIAAYagAAAAEE71ICXuDQF63s1uwa4b4OhrpwVtFjkl8XOqPJbHHzY6eXtSR3eyo0ICL3/gLRT3nA==", "NZDEABZRRXYI63UO3KKJXWAYNYHEOHZY", "admin@gmail.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "62052c97-c075-4141-8145-f44e25db7fd3", "AQAAAAIAAYagAAAAEAeQ0tpl2GJuQ5+4tDNpNaRchsPfJJZK7yCh6xHYjYhiyZ9XGk2J6gEQqPCc5kTVQQ==", "HW76SSA3WO3JGLMEP6SVI3477KKL4Z4A" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApiSettings",
                keyColumn: "ApiSettingsId",
                keyValue: 3);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "ApiSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ApiSettings",
                keyColumn: "ApiSettingsId",
                keyValue: 1,
                column: "SenderName",
                value: "Torun WOL");

            migrationBuilder.UpdateData(
                table: "ApiSettings",
                keyColumn: "ApiSettingsId",
                keyValue: 2,
                column: "SenderName",
                value: "Torun WOL");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "b227ff5c-3686-4b72-bd37-59d7b85aec15");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "de86a170-fcae-4c01-a6ba-ee12d1c190c3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "2d2ec0ea-258c-4f48-b061-b5edc53f4aac");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24195090-efc2-47d7-9bfb-4623e77f0cf8", "AQAAAAIAAYagAAAAENe5aPm81r8D/rBt2HT7S2mApuV9DZGKlNqREtawbBH69/5udyp6Y9WxoQaVaBzT5g==", "ES6KHRCNUCUSXDU7A46NLV4SN73OZAJJ" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "Email", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "48e915f3-46ec-4ee1-83bf-030338e6ff29", "admin@gmailcom", "AQAAAAIAAYagAAAAELxR5j/r4mNL7i9fT3zMQunHnSz0SkTcv88YmX/UcjkRF8PfRtbK8Mlv6VBViTRsAQ==", "ANIXGJTER5W4GFHS3MQBKITGSDBEA4CX", "admin@gmailcom" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b17e4e6-1326-4935-abde-2633e28a0f24", "AQAAAAIAAYagAAAAEDd01DVO4REoGjYq3dMlg8oFd1iF2X8P+GzgAr82vo41tvGqTUVa9AXcRUruDX2uPQ==", "BAYE35AUTAONZMSEVBKGIQISJDQVYH5J" });
        }
    }
}
