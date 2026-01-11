using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeTactics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class V101 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbilityInPokemon_Ability_AbilityId",
                table: "AbilityInPokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_MoveInPokemon_Move_MoveId",
                table: "MoveInPokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Pokemon_Sprite_SpriteId",
                table: "Pokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Stat_Pokemon_PokemonId",
                table: "Stat");

            migrationBuilder.DropIndex(
                name: "IX_Pokemon_SpriteId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "SpriteId",
                table: "Pokemon");

            migrationBuilder.AlterColumn<int>(
                name: "PokemonId",
                table: "Stat",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PokemonId",
                table: "Sprite",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sprite_PokemonId",
                table: "Sprite",
                column: "PokemonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AbilityInPokemon_Ability_AbilityId",
                table: "AbilityInPokemon",
                column: "AbilityId",
                principalTable: "Ability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoveInPokemon_Move_MoveId",
                table: "MoveInPokemon",
                column: "MoveId",
                principalTable: "Move",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sprite_Pokemon_PokemonId",
                table: "Sprite",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stat_Pokemon_PokemonId",
                table: "Stat",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbilityInPokemon_Ability_AbilityId",
                table: "AbilityInPokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_MoveInPokemon_Move_MoveId",
                table: "MoveInPokemon");

            migrationBuilder.DropForeignKey(
                name: "FK_Sprite_Pokemon_PokemonId",
                table: "Sprite");

            migrationBuilder.DropForeignKey(
                name: "FK_Stat_Pokemon_PokemonId",
                table: "Stat");

            migrationBuilder.DropIndex(
                name: "IX_Sprite_PokemonId",
                table: "Sprite");

            migrationBuilder.DropColumn(
                name: "PokemonId",
                table: "Sprite");

            migrationBuilder.AlterColumn<int>(
                name: "PokemonId",
                table: "Stat",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SpriteId",
                table: "Pokemon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_SpriteId",
                table: "Pokemon",
                column: "SpriteId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbilityInPokemon_Ability_AbilityId",
                table: "AbilityInPokemon",
                column: "AbilityId",
                principalTable: "Ability",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MoveInPokemon_Move_MoveId",
                table: "MoveInPokemon",
                column: "MoveId",
                principalTable: "Move",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pokemon_Sprite_SpriteId",
                table: "Pokemon",
                column: "SpriteId",
                principalTable: "Sprite",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stat_Pokemon_PokemonId",
                table: "Stat",
                column: "PokemonId",
                principalTable: "Pokemon",
                principalColumn: "Id");
        }
    }
}
