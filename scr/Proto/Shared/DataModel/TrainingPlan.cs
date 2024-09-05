namespace DataModel
{
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using DataModel.Attributes;
    using DataModel.Resources;

    public class TrainingPlan : IEntity
    {
        [Required(ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsRequired))]
        [MinLength(4, ErrorMessageResourceType = typeof(Errors), ErrorMessageResourceName = nameof(Errors.FieldIsToShort))]
        [MaxLength(50, ErrorMessageResourceType = typeof(Errors), ErrorMessage = nameof(Errors.FieldIsToLong))]
        public string Name { get; set; } = string.Empty;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [ForeignKey("AspNetUsers")]
        public string CustomerId { get; set; } = string.Empty;

        [SwaggerParameterIgnore]
        public Collection<Unit>? Units { get;  set; }
    }
}
