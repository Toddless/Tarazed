namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ExerciseSetExercise : IEntity, IMappingEntity
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(ExerciseSet))]
        public long ExerciseSetId { get; set; }

        [ForeignKey(nameof(Exercise))]
        public long ExerciseId { get; set; }

        public void CreateMapping(long souceId, long targetId)
        {
            ExerciseSetId = souceId;
            ExerciseId = targetId;
        }
    }
}
