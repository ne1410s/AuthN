using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthN.Persistence.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Privileges",
                columns: table => new
                {
                    PrivilegeId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Privileges", x => x.PrivilegeId);
                    table.UniqueConstraint("AK_Privileges_Type", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PasswordSalt = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Forename = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisteredEmail = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FacebookId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActivationCodeGeneratedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActivationCode = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AuthNPrivilegeAuthNUser",
                columns: table => new
                {
                    PrivilegesPrivilegeId = table.Column<int>(type: "int", nullable: false),
                    UsersUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthNPrivilegeAuthNUser", x => new { x.PrivilegesPrivilegeId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_AuthNPrivilegeAuthNUser_Privileges_PrivilegesPrivilegeId",
                        column: x => x.PrivilegesPrivilegeId,
                        principalTable: "Privileges",
                        principalColumn: "PrivilegeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthNPrivilegeAuthNUser_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Privileges",
                columns: new[] { "PrivilegeId", "Type" },
                values: new object[] { 0, "Default" });

            migrationBuilder.InsertData(
                table: "Privileges",
                columns: new[] { "PrivilegeId", "Type" },
                values: new object[] { 1, "AssignPrivileges" });

            migrationBuilder.InsertData(
                table: "Privileges",
                columns: new[] { "PrivilegeId", "Type" },
                values: new object[] { 2, "DeleteUser" });

            migrationBuilder.CreateIndex(
                name: "IX_AuthNPrivilegeAuthNUser_UsersUserId",
                table: "AuthNPrivilegeAuthNUser",
                column: "UsersUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegisteredEmail",
                table: "Users",
                column: "RegisteredEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthNPrivilegeAuthNUser");

            migrationBuilder.DropTable(
                name: "Privileges");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
