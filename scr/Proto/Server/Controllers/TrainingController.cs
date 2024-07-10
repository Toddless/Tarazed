namespace Server.Controllers
{
    using System.Threading.Tasks;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class TrainingController
        : AbstractRelationController<ExerciseSet,
          TrainingPlan,
          TrainingPlanExerciseSets>
    {
        public TrainingController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger<TrainingController> logger)
            : base(manager, context, logger)
        {
        }

        [HttpPut]
        public override Task<TrainingPlanExerciseSets?> CreateAsync(TrainingPlanExerciseSetsDto item)
        {

            var trainingsplan
            return base.CreateAsync(item.Export(), item.TrainingPlanId);
        }
    }

    class TrainingPlanExerciseSetsDto
    {
        public string ExcersiseSetName { get; set; }
        public DateTime CompletionDate { get; set; }

        public long TrainingPlanId { get; set; }

        public ExerciseSet Export()
        {
            return new ExerciseSet()
            {
                CompletionDate = this.CompletionDate,
                Name = this.ExcersiseSetName,
            }
        }
    }
}
