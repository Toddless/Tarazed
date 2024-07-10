namespace DataModel
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CustomerTrainingPlan : IEntity, IMappingEntity
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("AspNetUsers")]
        public long CustomerId { get; set; }

        [ForeignKey(nameof(TrainingPlan))]
        public long TrainingPlanId { get; set; }

        public TrainingPlan? TrainingPlan { get; set; }

        public void CreateMapping(long userId, long trainingsPlanId)
        {
            CustomerId = userId;
            TrainingPlanId = trainingsPlanId;
        }
    }
}
