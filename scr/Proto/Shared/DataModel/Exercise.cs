namespace DataModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DataModel.Resources;

    public class Exercise : IEntity, IHaveName
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(Workout))]
        public long WorkoutId { get; set; }

        [Range(1, 100)]
        public int Reps { get; set; }

        [Range(1, 10)]
        public int Set { get; set; }

        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsRequired))]
        [MinLength(4, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsToShort))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.FieldIsToLong))]
        public string Name { get; set; } = string.Empty;

        [Range(1, 1500)]
        public double Weight { get; set; }

        [MinLength(0, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.DescriptionIsToShort))]
        [MaxLength(800, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.DescriptionsIsToLong))]
        public string? Description { get; set; } = string.Empty;

        public string CustomerId { get; set; } = string.Empty;

        public Collection<MuscleIntensityLevel>? MuscleIntensityLevelId { get; set; }
    }
}
