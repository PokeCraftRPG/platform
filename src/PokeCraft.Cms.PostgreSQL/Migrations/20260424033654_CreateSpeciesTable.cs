using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeCraft.Cms.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateSpeciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "Pokemon",
                table: "Moves",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<byte>(
                name: "PowerPoints",
                schema: "Pokemon",
                table: "Moves",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<byte>(
                name: "Power",
                schema: "Pokemon",
                table: "Moves",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                schema: "Pokemon",
                table: "Moves",
                type: "character varying(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<byte>(
                name: "Accuracy",
                schema: "Pokemon",
                table: "Moves",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Species",
                schema: "Pokemon",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BaseFriendship = table.Column<byte>(type: "smallint", nullable: false),
                    CatchRate = table.Column<byte>(type: "smallint", nullable: false),
                    GrowthRate = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    EggCycles = table.Column<byte>(type: "smallint", nullable: false),
                    PrimaryEggGroup = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    SecondaryEggGroup = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.SpeciesId);
                });

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
                name: "IX_Species_CreatedBy",
                schema: "Pokemon",
                table: "Species",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Species_CreatedOn",
                schema: "Pokemon",
                table: "Species",
                column: "CreatedOn");

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
                name: "IX_Species_IsPublished",
                schema: "Pokemon",
                table: "Species",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Key",
                schema: "Pokemon",
                table: "Species",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_Name",
                schema: "Pokemon",
                table: "Species",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_Number",
                schema: "Pokemon",
                table: "Species",
                column: "Number",
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
                name: "IX_Species_StreamId",
                schema: "Pokemon",
                table: "Species",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_UniqueId",
                schema: "Pokemon",
                table: "Species",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Species_UpdatedBy",
                schema: "Pokemon",
                table: "Species",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Species_UpdatedOn",
                schema: "Pokemon",
                table: "Species",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Species_Version",
                schema: "Pokemon",
                table: "Species",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Species",
                schema: "Pokemon");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                schema: "Pokemon",
                table: "Moves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<int>(
                name: "PowerPoints",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "smallint");

            migrationBuilder.AlterColumn<int>(
                name: "Power",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                schema: "Pokemon",
                table: "Moves",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<int>(
                name: "Accuracy",
                schema: "Pokemon",
                table: "Moves",
                type: "integer",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "smallint",
                oldNullable: true);
        }
    }
}
