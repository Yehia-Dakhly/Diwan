using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diwan.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueToFriendshipAndReactionForeginKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reaction_UserId",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_RequesterId",
                table: "Friendship");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_UserId_PostId",
                table: "Reaction",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_RequesterId_AddresseeId",
                table: "Friendship",
                columns: new[] { "RequesterId", "AddresseeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reaction_UserId_PostId",
                table: "Reaction");

            migrationBuilder.DropIndex(
                name: "IX_Friendship_RequesterId_AddresseeId",
                table: "Friendship");

            migrationBuilder.CreateIndex(
                name: "IX_Reaction_UserId",
                table: "Reaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendship_RequesterId",
                table: "Friendship",
                column: "RequesterId");
        }
    }
}
