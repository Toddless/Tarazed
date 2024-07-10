namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TrainingPlanExerciseSets : IMappingEntity
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(ExerciseSet))]
        public long TrainingPlanId { get; set; }

        [ForeignKey(nameof(TrainingPlan))]
        public long ExerciseSetId { get; set; }

        public TrainingPlan? TrainingPlan { get; set; }

        public ExerciseSet? ExerciseSet { get; set; }

        public void CreateMapping(long exerciseSetId, long trainingsPlanId)
        {
            ExerciseSetId = exerciseSetId;
            TrainingPlanId = trainingsPlanId;
        }
    }
}
