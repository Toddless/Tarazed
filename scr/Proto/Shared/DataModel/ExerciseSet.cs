using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class ExerciseSet
    {
        public Exercise Exercises
        {
            get; set;
        }

        public string Name
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
