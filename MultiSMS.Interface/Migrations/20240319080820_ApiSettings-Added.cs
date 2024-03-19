using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class ApiSettingsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiSettings",
                columns: table => new
                {
                    ApiSettingsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiActive = table.Column<bool>(type: "bit", nullable: false),
                    FastChannel = table.Column<bool>(type: "bit", nullable: false),
                    TestMode = table.Column<bool>(type: "bit", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSettings", x => x.ApiSettingsId);
                });

            migrationBuilder.InsertData(
                table: "ApiSettings",
                columns: new[] { "ApiSettingsId", "ApiActive", "ApiName", "FastChannel", "SenderName", "TestMode" },
                values: new object[,]
                {
                    { 1, true, "ServerSms", true, "Torun WOL", true },
                    { 2, false, "SmsApi", true, "Torun WOL", true }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEA2yN2JkEhvV2vrI8nrhMj2ofJ5/sNSOJlFQ3cJFSDnHn0B2KgBdoFKUax8G7C0PCg==", "236WDMMLZL6IBZMN4TIY2GVKKJZ32HM7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiSettings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEG3c32It8qfqxm0jnDOIe1qmSMVMx2w2vyD++8VaR0dIsLtUXPAOiXYBXEmr/tVgpQ==", "VXXZSLL7H62O4WLQJV4TRMS5X25QG24W" });
        }
    }
}
