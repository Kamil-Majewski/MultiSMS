using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations
{
    /// <inheritdoc />
    public partial class Ch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SMSContent",
                table: "SMSMessageTemplates",
                newName: "TemplateContent");

            migrationBuilder.AddColumn<string>(
                name: "TemplateDescription",
                table: "SMSMessageTemplates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemplateDescription",
                table: "SMSMessageTemplates");

            migrationBuilder.RenameColumn(
                name: "TemplateContent",
                table: "SMSMessageTemplates",
                newName: "SMSContent");
        }
    }
}
