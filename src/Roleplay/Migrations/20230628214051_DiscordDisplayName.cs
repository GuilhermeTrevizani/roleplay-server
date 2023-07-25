using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roleplay.Migrations
{
    /// <inheritdoc />
    public partial class DiscordDisplayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscordDiscriminator",
                table: "Users",
                newName: "DiscordDisplayName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscordDisplayName",
                table: "Users",
                newName: "DiscordDiscriminator");
        }
    }
}
