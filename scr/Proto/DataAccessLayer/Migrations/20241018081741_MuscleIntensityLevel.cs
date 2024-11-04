#nullable disable

namespace DataAccessLayer.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class MuscleIntensityLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MuscleIntensityLevels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Intensity = table.Column<int>(type: "int", nullable: false),
                    Muscle = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscleIntensityLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MuscleIntensityLevels_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MuscleIntensityLevels_ExerciseId",
                table: "MuscleIntensityLevels",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MuscleIntensityLevels");
        }
    }
}
