namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using DataModel.Resources;

    public class ExerciseSet
    {
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.NameIsRequired))]
        [MinLength(5, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.NameIsToShort))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.NameIsToLong))]
        public string Name { get; set; } = string.Empty;

        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Date of completion in UTC ticks.
        /// </summary>
        public long? CompletionDate { get; set; }
    }
}
