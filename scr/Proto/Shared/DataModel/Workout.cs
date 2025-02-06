namespace DataModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DataModel.Attributes;
    using DataModel.Resources;

    public class Workout : IEntity, IHaveName
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

        [ForeignKey("TrainingPlans")]
        public long TrainingPlanId { get; set; } = 0;

        public string CustomerId { get; set; } = string.Empty;

        [SwaggerParameterIgnore]
        public Collection<Exercise>? Exercises { get; set; }
    }
}
