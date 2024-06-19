namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TrainingPlanExerciseSets
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(ExerciseSet))]
        public long TrainingPlanId { get; set; }

        [ForeignKey(nameof(TrainingPlan))]
        public long ExerciseSetId { get; set; }
    }
}
