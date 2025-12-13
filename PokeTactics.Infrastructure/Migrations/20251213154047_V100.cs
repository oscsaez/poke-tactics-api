using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeTactics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V100 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilitiesInPokemon");

            migrationBuilder.DropTable(
                name: "MovesInPokemon");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Ability");

            migrationBuilder.AlterColumn<string>(
                name: "OfficialArtworkUri",
                table: "Sprite",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "PowerPoints",
                table: "Move",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Move",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Move",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ability",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AbilityInPokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    AbilityId = table.Column<int>(type: "int", nullable: false),
                    IsHidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityInPokemon", x => new { x.PokemonId, x.AbilityId });
                    table.ForeignKey(
                        name: "FK_AbilityInPokemon_Ability_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Ability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AbilityInPokemon_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MoveInPokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    MoveId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoveInPokemon", x => new { x.PokemonId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_MoveInPokemon_Move_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Move",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoveInPokemon_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Sprite_OfficialArtworkUri",
                table: "Sprite",
                column: "OfficialArtworkUri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_Name",
                table: "Pokemon",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Move_Name",
                table: "Move",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ability_Name",
                table: "Ability",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbilityInPokemon_AbilityId",
                table: "AbilityInPokemon",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_MoveInPokemon_MoveId",
                table: "MoveInPokemon",
                column: "MoveId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilityInPokemon");

            migrationBuilder.DropTable(
                name: "MoveInPokemon");

            migrationBuilder.DropIndex(
                name: "IX_Sprite_OfficialArtworkUri",
                table: "Sprite");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_Name",
                table: "Pokemon");

            migrationBuilder.DropIndex(
                name: "IX_Move_Name",
                table: "Move");

            migrationBuilder.DropIndex(
                name: "IX_Ability_Name",
                table: "Ability");

            migrationBuilder.UpdateData(
                table: "Sprite",
                keyColumn: "OfficialArtworkUri",
                keyValue: null,
                column: "OfficialArtworkUri",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "OfficialArtworkUri",
                table: "Sprite",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "PowerPoints",
                table: "Move",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Move",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Move",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Ability",
                type: "varchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Ability",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AbilitiesInPokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    AbilityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilitiesInPokemon", x => new { x.PokemonId, x.AbilityId });
                    table.ForeignKey(
                        name: "FK_AbilitiesInPokemon_Ability_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Ability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbilitiesInPokemon_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MovesInPokemon",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "int", nullable: false),
                    MoveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovesInPokemon", x => new { x.PokemonId, x.MoveId });
                    table.ForeignKey(
                        name: "FK_MovesInPokemon_Move_MoveId",
                        column: x => x.MoveId,
                        principalTable: "Move",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovesInPokemon_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AbilitiesInPokemon_AbilityId",
                table: "AbilitiesInPokemon",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_MovesInPokemon_MoveId",
                table: "MovesInPokemon",
                column: "MoveId");
        }
    }
}
