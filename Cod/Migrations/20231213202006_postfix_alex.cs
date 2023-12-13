using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cod.Migrations
{
    public partial class postfix_alex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupProfile");

            migrationBuilder.CreateTable(
                name: "ApplicationUserGroup",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "int", nullable: false),
                    ProfilesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserGroup", x => new { x.GroupsId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserGroup_AspNetUsers_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserGroup_ProfilesId",
                table: "ApplicationUserGroup",
                column: "ProfilesId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserGroup");

            migrationBuilder.CreateTable(
                name: "GroupProfile",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "int", nullable: false),
                    ProfilesId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupProfile", x => new { x.GroupsId, x.ProfilesId });
                    table.ForeignKey(
                        name: "FK_GroupProfile_AspNetUsers_ProfilesId",
                        column: x => x.ProfilesId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupProfile_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupProfile_ProfilesId",
                table: "GroupProfile",
                column: "ProfilesId");
        }
    }
}
