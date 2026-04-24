using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PokeCraft.Cms.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateFormTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HasGenderDifferences = table.Column<bool>(type: "boolean", nullable: false),
                    IsBattleOnly = table.Column<bool>(type: "boolean", nullable: false),
                    IsMega = table.Column<bool>(type: "boolean", nullable: false),
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
                name: "IX_Forms_IsBattleOnly",
                schema: "Pokemon",
                table: "Forms",
                column: "IsBattleOnly");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_IsDefault",
                schema: "Pokemon",
                table: "Forms",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_IsMega",
                schema: "Pokemon",
                table: "Forms",
                column: "IsMega");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormAbilities",
                schema: "Pokemon");

            migrationBuilder.DropTable(
                name: "Forms",
                schema: "Pokemon");
        }
    }
}
