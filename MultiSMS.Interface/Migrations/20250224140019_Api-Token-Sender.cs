using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class ApiTokenSender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "f8adc7ed-c42f-4d42-8fa5-b1c1b47e5ea7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "14686f02-1e3e-47ec-969b-7e20f92f082f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "e2c815ac-ac28-4b65-aa53-f735905e382a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "766c436b-bbbc-4592-a827-63af1cb23e34", "AQAAAAIAAYagAAAAED4QVN2XZKL1f2y2Z9KbfYjAHKCR35gyxxSJ3FZnNc2beZ8LZAG/CpdQ7KDJovcFdg==", "NXZG34IA6USHZUING7HNNSLG2P2ZZBLO" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cc420b94-896e-4744-8101-37ac2787bd62", "AQAAAAIAAYagAAAAEO5Xk+5QuC3bpQXtOWM99EvCVf9oOX8UO5VOjTZ68soo/0zY4ynhD/KWoe0XBm9xyw==", "3XE4QKZBU475FOBDG3LEFTZ5GD2Z6EFR" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c30708aa-7b10-4b2a-8acf-205ea71e009e", "AQAAAAIAAYagAAAAEL1n1t8i6VNi8AKmEmFOUW+sthIiISF17q+PdnLG3r27V5KZZaett0vePLTq6eD3AQ==", "NQHDOZXPA2FKN3WQNWKOAXMINLL3XKSC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c0e735de-3290-4692-b780-fc8f0ac89602", "AQAAAAIAAYagAAAAEE71ICXuDQF63s1uwa4b4OhrpwVtFjkl8XOqPJbHHzY6eXtSR3eyo0ICL3/gLRT3nA==", "NZDEABZRRXYI63UO3KKJXWAYNYHEOHZY" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "62052c97-c075-4141-8145-f44e25db7fd3", "AQAAAAIAAYagAAAAEAeQ0tpl2GJuQ5+4tDNpNaRchsPfJJZK7yCh6xHYjYhiyZ9XGk2J6gEQqPCc5kTVQQ==", "HW76SSA3WO3JGLMEP6SVI3477KKL4Z4A" });
        }
    }
}
