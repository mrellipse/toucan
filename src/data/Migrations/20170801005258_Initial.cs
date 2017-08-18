using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "user_seq");

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    ProviderId = table.Column<string>(type: "varchar(64)", nullable: false),
                    Description = table.Column<string>(type: "varchar(512)", nullable: false),
                    Enabled = table.Column<bool>(nullable: false, defaultValueSql: "true"),
                    Name = table.Column<string>(type: "varchar(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.ProviderId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp"),
                    DisplayName = table.Column<string>(type: "varchar(128)", nullable: false),
                    Enabled = table.Column<bool>(nullable: false, defaultValueSql: "true"),
                    Username = table.Column<string>(type: "varchar(128)", nullable: false),
                    Verified = table.Column<bool>(nullable: false, defaultValueSql: "false")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "varchar(16)", nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp"),
                    Enabled = table.Column<bool>(nullable: false, defaultValueSql: "false"),
                    Name = table.Column<string>(type: "varchar(64)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_Role_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProvider",
                columns: table => new
                {
                    ProviderId = table.Column<string>(type: "varchar(64)", nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp"),
                    ExternalId = table.Column<string>(type: "varchar(64)", nullable: true),
                    UserProviderType = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(256)", nullable: true),
                    PasswordSalt = table.Column<string>(type: "varchar(128)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProvider", x => new { x.ProviderId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserProvider_Provider",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProvider_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Verification",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    Code = table.Column<string>(type: "varchar(64)", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "current_timestamp"),
                    RedeemedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verification", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Verification_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "varchar(16)", nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.RoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRole_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedBy",
                table: "Role",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserProvider_UserId",
                table: "UserProvider",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProvider");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Verification");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropSequence(
                name: "user_seq");
        }
    }
}
