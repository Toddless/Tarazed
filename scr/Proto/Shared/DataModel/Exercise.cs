namespace DataModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

    public class Exercise : IEntity, IHaveName
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsRequired))]
        [MinLength(4, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsToShort))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.FieldIsToLong))]
        public string Name { get; set; } = string.Empty;

        [MinLength(0, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.DescriptionIsToShort))]
        [MaxLength(800, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.DescriptionsIsToLong))]
        public string? Description { get; set; } = string.Empty;

        public Collection<MuscleIntensityLevel>? MuscleIntensityLevelId { get; set; }

        public string CustomerId { get; set; } = string.Empty;
    }

    public class SetTemplate : IEntity
    {
        [Key]
        public long Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public int ExerciseId { get; set; }

        public int Index { get; set; }

        [Range(1, 1500)]
        public double Value { get; set; }

        public MesurementUnit MesurementUnit { get; set; }

        [Range(1, 100)]
        public int? Reps { get; set; }
    }

    public class Set : IEntity
    {
        [Key]
        public long Id { get; set; }

        public string CustomerId { get; set; } = string.Empty;

        public int ExerciseId { get; set; }

        public int Index { get; set; }

        [Range(1, 1500)]
        public double Value { get; set; }

        public MesurementUnit MesurementUnit { get; set; }

        [Range(1, 100)]
        public int? Reps { get; set; }

        public DateTime? Completed { get; set; }
    }
}
