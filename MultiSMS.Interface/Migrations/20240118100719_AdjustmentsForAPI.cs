using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class AdjustmentsForAPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_SMSMessages_SMSMessageSMSId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_SMSMessages_Groups_ChosenGroupGroupId",
                table: "SMSMessages");

            migrationBuilder.DropIndex(
                name: "IX_SMSMessages_ChosenGroupGroupId",
                table: "SMSMessages");

            migrationBuilder.DropIndex(
                name: "IX_Employees_SMSMessageSMSId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SMSMessageSMSId",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "SMSContent",
                table: "SMSMessages",
                newName: "ServerResponseSerialized");

            migrationBuilder.RenameColumn(
                name: "Issuer",
                table: "SMSMessages",
                newName: "DataDictionarySerialized");

            migrationBuilder.RenameColumn(
                name: "ChosenGroupGroupId",
                table: "SMSMessages",
                newName: "IssuerId");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalPhoneNumbers",
                table: "SMSMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChosenGroupId",
                table: "SMSMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEBCjKOKL7JXJsx7QCT7qvvJruPt1yUnTwn5xGiOieq6J8drhwl3gJKZDeM9bcwx0sQ==", "R5OAZQNRVKX2NIGZ6L6WJPNQ3K4LW23F" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalPhoneNumbers",
                table: "SMSMessages");

            migrationBuilder.DropColumn(
                name: "ChosenGroupId",
                table: "SMSMessages");

            migrationBuilder.RenameColumn(
                name: "ServerResponseSerialized",
                table: "SMSMessages",
                newName: "SMSContent");

            migrationBuilder.RenameColumn(
                name: "IssuerId",
                table: "SMSMessages",
                newName: "ChosenGroupGroupId");

            migrationBuilder.RenameColumn(
                name: "DataDictionarySerialized",
                table: "SMSMessages",
                newName: "Issuer");

            migrationBuilder.AddColumn<int>(
                name: "SMSMessageSMSId",
                table: "Employees",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "SecurityStamp" },
                values: new object[] { "AQAAAAIAAYagAAAAEC+Rbzml8UViUvgYaB/B4Us2o2weIzHxKIBdtBiT4fGNSZuLtf09nrx/sPYNijbQQw==", "WLGMYNDY6WWMVVYNN5JWGMTA6FDVC5GD" });

            migrationBuilder.CreateIndex(
                name: "IX_SMSMessages_ChosenGroupGroupId",
                table: "SMSMessages",
                column: "ChosenGroupGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_SMSMessageSMSId",
                table: "Employees",
                column: "SMSMessageSMSId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_SMSMessages_SMSMessageSMSId",
                table: "Employees",
                column: "SMSMessageSMSId",
                principalTable: "SMSMessages",
                principalColumn: "SMSId");

            migrationBuilder.AddForeignKey(
                name: "FK_SMSMessages_Groups_ChosenGroupGroupId",
                table: "SMSMessages",
                column: "ChosenGroupGroupId",
                principalTable: "Groups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
