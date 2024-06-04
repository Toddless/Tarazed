namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomerTrainingPlan
    {
        [Key]
        public long Id
        {
            get; set;
        }

        [ForeignKey(nameof(Customer))]
        public long CustomerId
        {
            get; set;
        }

        [ForeignKey(nameof(TrainingPlan))]
        public long TrainingPlanId
        {
            get; set;
        }
    }
}
