namespace DataModel
{
    using System.ComponentModel.DataAnnotations;

    public class Exercise
    {
        [Range(0, 100)]
        public int Reps
        {
            get; set;
        }

        [Range(0, 10)]
        public int Set
        {
            get; set;
        }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 1500)]
        public double Weight
        {
            get; set;
        }

        [Key]
        public long Id
        {
            get; set;
        }

        [Required]
        [MinLength(5)]
        [MaxLength(800)]
        public string Description { get; set; } = string.Empty;
    }
}
