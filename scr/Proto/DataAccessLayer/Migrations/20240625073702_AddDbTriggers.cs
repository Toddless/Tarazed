#nullable disable

namespace DataAccessLayer.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddDbTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanExerciseSets_ExerciseSetId",
                table: "TrainingPlanExerciseSets",
                column: "ExerciseSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanExerciseSets_TrainingPlanId",
                table: "TrainingPlanExerciseSets",
                column: "TrainingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSetsExercise_ExerciseId",
                table: "ExerciseSetsExercise",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSetsExercise_ExerciseSetId",
                table: "ExerciseSetsExercise",
                column: "ExerciseSetId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTrainingPlans_CustomerId",
                table: "CustomerTrainingPlans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTrainingPlans_TrainingPlanId",
                table: "CustomerTrainingPlans",
                column: "TrainingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTrainingPlans_Customers_CustomerId",
                table: "CustomerTrainingPlans",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTrainingPlans_TrainingPlans_TrainingPlanId",
                table: "CustomerTrainingPlans",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSetsExercise_ExercisePlans_ExerciseId",
                table: "ExerciseSetsExercise",
                column: "ExerciseId",
                principalTable: "ExercisePlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseSetsExercise_ExerciseSets_ExerciseSetId",
                table: "ExerciseSetsExercise",
                column: "ExerciseSetId",
                principalTable: "ExerciseSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlanExerciseSets_ExerciseSets_ExerciseSetId",
                table: "TrainingPlanExerciseSets",
                column: "ExerciseSetId",
                principalTable: "ExerciseSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingPlanExerciseSets_TrainingPlans_TrainingPlanId",
                table: "TrainingPlanExerciseSets",
                column: "TrainingPlanId",
                principalTable: "TrainingPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTrainingPlans_Customers_CustomerId",
                table: "CustomerTrainingPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTrainingPlans_TrainingPlans_TrainingPlanId",
                table: "CustomerTrainingPlans");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSetsExercise_ExercisePlans_ExerciseId",
                table: "ExerciseSetsExercise");

            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseSetsExercise_ExerciseSets_ExerciseSetId",
                table: "ExerciseSetsExercise");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlanExerciseSets_ExerciseSets_ExerciseSetId",
                table: "TrainingPlanExerciseSets");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingPlanExerciseSets_TrainingPlans_TrainingPlanId",
                table: "TrainingPlanExerciseSets");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPlanExerciseSets_ExerciseSetId",
                table: "TrainingPlanExerciseSets");

            migrationBuilder.DropIndex(
                name: "IX_TrainingPlanExerciseSets_TrainingPlanId",
                table: "TrainingPlanExerciseSets");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseSetsExercise_ExerciseId",
                table: "ExerciseSetsExercise");

            migrationBuilder.DropIndex(
                name: "IX_ExerciseSetsExercise_ExerciseSetId",
                table: "ExerciseSetsExercise");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTrainingPlans_CustomerId",
                table: "CustomerTrainingPlans");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTrainingPlans_TrainingPlanId",
                table: "CustomerTrainingPlans");
        }
    }
}
