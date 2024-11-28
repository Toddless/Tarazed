namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UnitExercise
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Exercise))]
        public long ExerciseId { get; set; }

        [ForeignKey(nameof(Unit))]
        public long UnitId { get; set; }
    }
}
