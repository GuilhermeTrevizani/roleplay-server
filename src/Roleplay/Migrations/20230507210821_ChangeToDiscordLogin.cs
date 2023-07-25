using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleplay.Migrations
{
    /// <inheritdoc />
    public partial class ChangeToDiscordLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discord",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DiscordConfirmationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "ResetPasswordToken",
                table: "Users",
                newName: "DiscordUsername");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "DiscordId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Users",
                newName: "DiscordDiscriminator");

            migrationBuilder.AddColumn<bool>(
                name: "AnsweredQuestions",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnsweredQuestions",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DiscordUsername",
                table: "Users",
                newName: "ResetPasswordToken");

            migrationBuilder.RenameColumn(
                name: "DiscordId",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "DiscordDiscriminator",
                table: "Users",
                newName: "Name");

            migrationBuilder.AddColumn<ulong>(
                name: "Discord",
                table: "Users",
                type: "bigint unsigned",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<string>(
                name: "DiscordConfirmationToken",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
