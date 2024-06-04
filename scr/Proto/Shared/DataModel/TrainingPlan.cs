namespace DataModel
{
    using System.ComponentModel.DataAnnotations;

    public class TrainingPlan
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
    }
}
