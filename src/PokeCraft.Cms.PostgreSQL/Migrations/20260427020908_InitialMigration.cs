using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeCraft.Cms.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Pokemon");

            migrationBuilder.CreateTable(
                name: "Abilities",
                schema: "Pokemon",
                columns: table => new
                {
                    AbilityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.AbilityId);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                schema: "Pokemon",
                columns: table => new
                {
                    MoveId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Category = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Accuracy = table.Column<byte>(type: "smallint", nullable: true),
                    Power = table.Column<byte>(type: "smallint", nullable: true),
                    PowerPoints = table.Column<byte>(type: "smallint", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => x.MoveId);
                });

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

            migrationBuilder.CreateTable(
                name: "Varieties",
                schema: "Pokemon",
                columns: table => new
                {
                    VarietyId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Genus = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CanChangeForm = table.Column<bool>(type: "boolean", nullable: false),
                    GenderRatio = table.Column<byte>(type: "smallint", nullable: true),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Varieties", x => x.VarietyId);
                    table.ForeignKey(
                        name: "FK_Varieties_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalSchema: "Pokemon",
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                schema: "Pokemon",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    VarietyId = table.Column<int>(type: "integer", nullable: false),
                    Kind = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HasGenderDifferences = table.Column<bool>(type: "boolean", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    PrimaryType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    SecondaryType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    BaseHP = table.Column<byte>(type: "smallint", nullable: false),
                    BaseAttack = table.Column<byte>(type: "smallint", nullable: false),
                    BaseDefense = table.Column<byte>(type: "smallint", nullable: false),
                    BaseSpecialAttack = table.Column<byte>(type: "smallint", nullable: false),
                    BaseSpecialDefense = table.Column<byte>(type: "smallint", nullable: false),
                    BaseSpeed = table.Column<byte>(type: "smallint", nullable: false),
                    YieldExperience = table.Column<int>(type: "integer", nullable: false),
                    YieldHP = table.Column<byte>(type: "smallint", nullable: false),
                    YieldAttack = table.Column<byte>(type: "smallint", nullable: false),
                    YieldDefense = table.Column<byte>(type: "smallint", nullable: false),
                    YieldSpecialAttack = table.Column<byte>(type: "smallint", nullable: false),
                    YieldSpecialDefense = table.Column<byte>(type: "smallint", nullable: false),
                    YieldSpeed = table.Column<byte>(type: "smallint", nullable: false),
                    StreamId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                    table.ForeignKey(
                        name: "FK_Forms_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VarietyMoves",
                schema: "Pokemon",
                columns: table => new
                {
                    VarietyId = table.Column<int>(type: "integer", nullable: false),
                    MoveId = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarietyMoves", x => new { x.VarietyId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_VarietyMoves_Moves_MoveId",
                        column: x => x.MoveId,
                        principalSchema: "Pokemon",
                        principalTable: "Moves",
                        principalColumn: "MoveId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VarietyMoves_Varieties_VarietyId",
                        column: x => x.VarietyId,
                        principalSchema: "Pokemon",
                        principalTable: "Varieties",
                        principalColumn: "VarietyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormAbilities",
                schema: "Pokemon",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "integer", nullable: false),
                    AbilityId = table.Column<int>(type: "integer", nullable: false),
                    Slot = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAbilities", x => new { x.FormId, x.AbilityId });
                    table.ForeignKey(
                        name: "FK_FormAbilities_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalSchema: "Pokemon",
                        principalTable: "Abilities",
                        principalColumn: "AbilityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormAbilities_Forms_FormId",
                        column: x => x.FormId,
                        principalSchema: "Pokemon",
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_CreatedBy",
                schema: "Pokemon",
                table: "Abilities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_CreatedOn",
                schema: "Pokemon",
                table: "Abilities",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_IsPublished",
                schema: "Pokemon",
                table: "Abilities",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Key",
                schema: "Pokemon",
                table: "Abilities",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Name",
                schema: "Pokemon",
                table: "Abilities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_StreamId",
                schema: "Pokemon",
                table: "Abilities",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UniqueId",
                schema: "Pokemon",
                table: "Abilities",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UpdatedBy",
                schema: "Pokemon",
                table: "Abilities",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_UpdatedOn",
                schema: "Pokemon",
                table: "Abilities",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_Version",
                schema: "Pokemon",
                table: "Abilities",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_AbilityId",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_FormAbilities_Slot",
                schema: "Pokemon",
                table: "FormAbilities",
                column: "Slot");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CreatedBy",
                schema: "Pokemon",
                table: "Forms",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_CreatedOn",
                schema: "Pokemon",
                table: "Forms",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_HasGenderDifferences",
                schema: "Pokemon",
                table: "Forms",
                column: "HasGenderDifferences");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Height",
                schema: "Pokemon",
                table: "Forms",
                column: "Height");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_IsPublished",
                schema: "Pokemon",
                table: "Forms",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Key",
                schema: "Pokemon",
                table: "Forms",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Kind",
                schema: "Pokemon",
                table: "Forms",
                column: "Kind");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Name",
                schema: "Pokemon",
                table: "Forms",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_PrimaryType",
                schema: "Pokemon",
                table: "Forms",
                column: "PrimaryType");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_SecondaryType",
                schema: "Pokemon",
                table: "Forms",
                column: "SecondaryType");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_StreamId",
                schema: "Pokemon",
                table: "Forms",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UniqueId",
                schema: "Pokemon",
                table: "Forms",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UpdatedBy",
                schema: "Pokemon",
                table: "Forms",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UpdatedOn",
                schema: "Pokemon",
                table: "Forms",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_VarietyId",
                schema: "Pokemon",
                table: "Forms",
                column: "VarietyId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Version",
                schema: "Pokemon",
                table: "Forms",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Weight",
                schema: "Pokemon",
                table: "Forms",
                column: "Weight");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_YieldExperience",
                schema: "Pokemon",
                table: "Forms",
                column: "YieldExperience");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Accuracy",
                schema: "Pokemon",
                table: "Moves",
                column: "Accuracy");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Category",
                schema: "Pokemon",
                table: "Moves",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_CreatedBy",
                schema: "Pokemon",
                table: "Moves",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_CreatedOn",
                schema: "Pokemon",
                table: "Moves",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_IsPublished",
                schema: "Pokemon",
                table: "Moves",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Key",
                schema: "Pokemon",
                table: "Moves",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Name",
                schema: "Pokemon",
                table: "Moves",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Power",
                schema: "Pokemon",
                table: "Moves",
                column: "Power");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_PowerPoints",
                schema: "Pokemon",
                table: "Moves",
                column: "PowerPoints");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_StreamId",
                schema: "Pokemon",
                table: "Moves",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Type",
                schema: "Pokemon",
                table: "Moves",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UniqueId",
                schema: "Pokemon",
                table: "Moves",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UpdatedBy",
                schema: "Pokemon",
                table: "Moves",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_UpdatedOn",
                schema: "Pokemon",
                table: "Moves",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Moves_Version",
                schema: "Pokemon",
                table: "Moves",
                column: "Version");

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
                column: "EggCycles");

            migrationBuilder.CreateIndex(
                name: "IX_Species_GrowthRate",
                schema: "Pokemon",
                table: "Species",
                column: "GrowthRate");

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
                column: "Name");

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
                column: "PrimaryEggGroup");

            migrationBuilder.CreateIndex(
                name: "IX_Species_SecondaryEggGroup",
                schema: "Pokemon",
                table: "Species",
                column: "SecondaryEggGroup");

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

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_CanChangeForm",
                schema: "Pokemon",
                table: "Varieties",
                column: "CanChangeForm");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_CreatedBy",
                schema: "Pokemon",
                table: "Varieties",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_CreatedOn",
                schema: "Pokemon",
                table: "Varieties",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_GenderRatio",
                schema: "Pokemon",
                table: "Varieties",
                column: "GenderRatio");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Genus",
                schema: "Pokemon",
                table: "Varieties",
                column: "Genus");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_IsDefault",
                schema: "Pokemon",
                table: "Varieties",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_IsPublished",
                schema: "Pokemon",
                table: "Varieties",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Key",
                schema: "Pokemon",
                table: "Varieties",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Name",
                schema: "Pokemon",
                table: "Varieties",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_SpeciesId",
                schema: "Pokemon",
                table: "Varieties",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_StreamId",
                schema: "Pokemon",
                table: "Varieties",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UniqueId",
                schema: "Pokemon",
                table: "Varieties",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UpdatedBy",
                schema: "Pokemon",
                table: "Varieties",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_UpdatedOn",
                schema: "Pokemon",
                table: "Varieties",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Varieties_Version",
                schema: "Pokemon",
                table: "Varieties",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_Level",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_Method",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "Method");

            migrationBuilder.CreateIndex(
                name: "IX_VarietyMoves_MoveId",
                schema: "Pokemon",
                table: "VarietyMoves",
                column: "MoveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormAbilities",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "VarietyMoves",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Abilities",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Moves",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Varieties",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Species",
                schema: "Pokemon");
        }
    }
}
