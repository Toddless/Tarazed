#nullable disable

namespace DataAccessLayer.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSetsExercise_ExercisePlans_ExerciseId",
                table: "ExerciseSetsExercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExercisePlans",
                table: "ExercisePlans");

            migrationBuilder.RenameTable(
                name: "ExercisePlans",
                newName: "Exercises");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSetsExercise_Exercises_ExerciseId",
                table: "ExerciseSetsExercise",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSetsExercise_Exercises_ExerciseId",
                table: "ExerciseSetsExercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises");

            migrationBuilder.RenameTable(
                name: "Exercises",
                newName: "ExercisePlans");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExercisePlans",
                table: "ExercisePlans",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSetsExercise_ExercisePlans_ExerciseId",
                table: "ExerciseSetsExercise",
                column: "ExerciseId",
                principalTable: "ExercisePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
