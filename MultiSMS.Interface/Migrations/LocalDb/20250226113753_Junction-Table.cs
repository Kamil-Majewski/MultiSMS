using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiSMS.Interface.Migrations.LocalDb
{
    /// <inheritdoc />
    public partial class JunctionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssingedUserIds",
                table: "ApiSmsSenders");

            migrationBuilder.CreateTable(
                name: "ApiSmsSenderUsers",
                columns: table => new
                {
                    ApiSmsSenderId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiSmsSenderUsers", x => new { x.ApiSmsSenderId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ApiSmsSenderUsers_ApiSmsSenders_ApiSmsSenderId",
                        column: x => x.ApiSmsSenderId,
                        principalTable: "ApiSmsSenders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiSmsSenderUsers_UserId",
                table: "ApiSmsSenderUsers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiSmsSenderUsers");

            migrationBuilder.AddColumn<string>(
                name: "AssingedUserIds",
                table: "ApiSmsSenders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
