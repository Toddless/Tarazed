namespace DataModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

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
