using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeCraft.Cms.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Species_BaseFriendship",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_CatchRate",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_Category",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_GrowthRate",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_Name",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Moves_Name",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Abilities_Name",
                schema: "Pokemon",
                table: "Abilities");

            migrationBuilder.CreateIndex(
                name: "IX_Species_BaseFriendship",
                schema: "Pokemon",
                table: "Species",
                column: "BaseFriendship");

            migrationBuilder.CreateIndex(
                name: "IX_Species_CatchRate",
                schema: "Pokemon",
                table: "Species",
                column: "CatchRate");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Category",
                schema: "Pokemon",
                table: "Species",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species",
                column: "EggCycles");

            migrationBuilder.CreateIndex(
                name: "IX_Species_GrowthRate",
                schema: "Pokemon",
                table: "Species",
                column: "GrowthRate");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                schema: "Pokemon",
                table: "Species",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "PrimaryEggGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "SecondaryEggGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Name",
                schema: "Pokemon",
                table: "Moves",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Name",
                schema: "Pokemon",
                table: "Abilities",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Species_BaseFriendship",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_CatchRate",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_Category",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_GrowthRate",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_Name",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species");

            migrationBuilder.DropIndex(
                name: "IX_Moves_Name",
                schema: "Pokemon",
                table: "Moves");

            migrationBuilder.DropIndex(
                name: "IX_Abilities_Name",
                schema: "Pokemon",
                table: "Abilities");

            migrationBuilder.CreateIndex(
                name: "IX_Species_BaseFriendship",
                schema: "Pokemon",
                table: "Species",
                column: "BaseFriendship",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_CatchRate",
                schema: "Pokemon",
                table: "Species",
                column: "CatchRate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_Category",
                schema: "Pokemon",
                table: "Species",
                column: "Category",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_EggCycles",
                schema: "Pokemon",
                table: "Species",
                column: "EggCycles",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_GrowthRate",
                schema: "Pokemon",
                table: "Species",
                column: "GrowthRate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                schema: "Pokemon",
                table: "Species",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_PrimaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "PrimaryEggGroup",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "SecondaryEggGroup",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Name",
                schema: "Pokemon",
                table: "Moves",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Name",
                schema: "Pokemon",
                table: "Abilities",
                column: "Name",
                unique: true);
        }
    }
}
