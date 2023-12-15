using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cod.Migrations
{
    public partial class what : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_ProfileId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_AspNetUsers_ProfileId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Groups_GroupId",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Post",
                table: "Post");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "Post",
                newName: "Posts");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_Post_ProfileId",
                table: "Posts",
                newName: "IX_Posts_ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_GroupId",
                table: "Posts",
                newName: "IX_Posts_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ProfileId",
                table: "Comments",
                newName: "IX_Comments_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ProfileId",
                table: "Comments",
                column: "ProfileId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_ProfileId",
                table: "Posts",
                column: "ProfileId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Groups_GroupId",
                table: "Posts",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ProfileId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_ProfileId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Groups_GroupId",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                table: "Posts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Posts",
                newName: "Post");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_ProfileId",
                table: "Post",
                newName: "IX_Post_ProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_GroupId",
                table: "Post",
                newName: "IX_Post_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ProfileId",
                table: "Comment",
                newName: "IX_Comment_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Post",
                table: "Post",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_ProfileId",
                table: "Comment",
                column: "ProfileId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_AspNetUsers_ProfileId",
                table: "Post",
                column: "ProfileId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Groups_GroupId",
                table: "Post",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
