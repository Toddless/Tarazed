namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomerTrainingPlan : IEntity
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("AspNetUsers")]
        public string CustomerId { get; set; }

        [ForeignKey(nameof(TrainingPlan))]
        public long TrainingPlanId { get; set; }

        public TrainingPlan? TrainingPlan { get; set; }
    }
}
