using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel
{
    public class CustomerTrainingPlan
    {
        public long Id
        {
            get => default;
            set
            {
            }
        }

        [ForeignKey(nameof(Customer))]
        public long CustomerId
        {
            get => default;
            set
            {
            }
        }

        public long TrainingPlanId
        {
            get => default;
            set
            {
            }
        }
    }
}
