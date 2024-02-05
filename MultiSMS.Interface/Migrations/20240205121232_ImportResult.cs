using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class ImportResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Imports",
                columns: table => new
                {
                    ImportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImportStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImportMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedEmployeesSerialized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedEmployeesSerialized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvalidEmployeesSerialized = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NonExistantGroupIdsSerialized = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Imports", x => x.ImportId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEEnMhlKHlSb0yZlOas5jY4h+as1Je1Dt0EcWEgDt+rKSMjHQPFWf+BACzdqmg51X5g==", "RJWDQFKUESWKVBKOH67VZ6HC2NJ2NIVV" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Imports");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEGwKrUXc/sL0IW2E5+vXha2eJygeuYWXgP2Oaz360KqMwjNAB3juPbPWHOO7t8Y0lQ==", "TNBCLGFCJYR5XXCWQ6H3QE654HLJRIFN" });
        }
    }
}
