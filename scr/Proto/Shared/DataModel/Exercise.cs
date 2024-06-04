using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class Exercise
    {
        public int Reps
        {
            get;set;
        }

        public int Set
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public double Weight
        {
            get; set;
        }

        [Key]
        public long Id
        {
            get => default;
            set
            {
            }
        }
    }
}
