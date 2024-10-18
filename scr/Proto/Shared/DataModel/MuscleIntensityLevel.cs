namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class MuscleIntensityLevel : IEntity
    {
        [Key]
        public long Id { get; set; }

        public Intensity Intensity { get; set; }

        public Muscle Muscle { get; set; }

        [ForeignKey(nameof(Exercise))]
        public long ExerciseId { get; set; }

        public string CustomerId { get; set; } = string.Empty;
    }
}
