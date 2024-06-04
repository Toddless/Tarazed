namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ExerciseSetExercise
    {
        [Key]
        public long Id
        {
            get; set;
        }

        [ForeignKey(nameof(ExerciseSet))]
        public long ExerciseSetId
        {
            get; set;
        }

        [ForeignKey(nameof(Exercise))]
        public long ExerciseId
        {
            get; set;
        }
    }
}
