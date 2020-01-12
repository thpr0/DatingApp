using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingAPP.API.Migrations
{
    public partial class AddPublicID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_users_UserId",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Photos",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Photos_UserId",
                table: "Photos",
                newName: "IX_Photos_UserID");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Photos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Photos",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_users_UserID",
                table: "Photos",
                column: "UserID",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_users_UserID",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Photos");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Photos",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Photos_UserID",
                table: "Photos",
                newName: "IX_Photos_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Photos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
