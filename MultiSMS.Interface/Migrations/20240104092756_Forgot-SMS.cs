using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class ForgotSMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SMSMessageSMSId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SMSMessages",
                columns: table => new
                {
                    SMSId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SMSContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChosenGroupGroupId = table.Column<int>(type: "int", nullable: false),
                    MessageSentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSMessages", x => x.SMSId);
                    table.ForeignKey(
                        name: "FK_SMSMessages_Groups_ChosenGroupGroupId",
                        column: x => x.ChosenGroupGroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SMSMessageSMSId",
                table: "Employees",
                column: "SMSMessageSMSId");

            migrationBuilder.CreateIndex(
                name: "IX_SMSMessages_ChosenGroupGroupId",
                table: "SMSMessages",
                column: "ChosenGroupGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_SMSMessages_SMSMessageSMSId",
                table: "Employees",
                column: "SMSMessageSMSId",
                principalTable: "SMSMessages",
                principalColumn: "SMSId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_SMSMessages_SMSMessageSMSId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "SMSMessages");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SMSMessageSMSId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SMSMessageSMSId",
                table: "Employees");
        }
    }
}
