using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel
{
    public class TrainingPlanExerciseSets
    {
        public long Id
        {
            get => default;
            set
            {
            }
        }

        [ForeignKey(nameof(ExerciseSet))]
        public long TrainingPlanId
        {
            get => default;
            set
            {
            }
        }

        [ForeignKey(nameof(TrainingPlan))]
        public long ExerciseSetId
        {
            get => default;
            set
            {
            }
        }
    }
}
