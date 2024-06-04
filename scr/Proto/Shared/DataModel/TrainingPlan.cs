using System.ComponentModel.DataAnnotations;

namespace DataModel
{
    public class TrainingPlan
    {


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
