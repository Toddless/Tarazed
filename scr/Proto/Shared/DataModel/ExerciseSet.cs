namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

    public class ExerciseSet : IEntity
    {
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsRequired))]
        [MinLength(4, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsToShort))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.FieldIsToLong))]
        public string Name { get; set; } = string.Empty;

        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Date of completion in UTC ticks.
        /// </summary>
        public long? CompletionDate { get; set; }
    }
}
