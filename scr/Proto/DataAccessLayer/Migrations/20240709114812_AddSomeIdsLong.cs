#nullable disable

namespace DataAccessLayer.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddSomeIdsLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTrainingPlans_AspNetUsers_CustomerId",
                table: "CustomerTrainingPlans");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TrainingPlans",
                newName: "Ids");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ExerciseSetsExercise",
                newName: "Ids");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ExerciseSets",
                newName: "Ids");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Exercises",
                newName: "Ids");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CustomerTrainingPlans",
                newName: "Ids");

            migrationBuilder.AlterColumn<long>(
                name: "CustomerId",
                table: "CustomerTrainingPlans",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "Ids",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Relational:ColumnOrder", 1)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_AspNetUsers_Ids",
                table: "AspNetUsers",
                column: "Ids");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTrainingPlans_AspNetUsers_CustomerId",
                table: "CustomerTrainingPlans",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Ids",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTrainingPlans_AspNetUsers_CustomerId",
                table: "CustomerTrainingPlans");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_AspNetUsers_Ids",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Ids",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Ids",
                table: "TrainingPlans",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Ids",
                table: "ExerciseSetsExercise",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Ids",
                table: "ExerciseSets",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Ids",
                table: "Exercises",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Ids",
                table: "CustomerTrainingPlans",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "CustomerTrainingPlans",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTrainingPlans_AspNetUsers_CustomerId",
                table: "CustomerTrainingPlans",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
