namespace DataModel
{
    using System.ComponentModel.DataAnnotations;

    public class ExerciseSet
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Key]
        public long Id
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Date of completion in UTC ticks.
        /// </summary>
        public long? CompletionDate
        {
            get; set;
        }
    }
}
