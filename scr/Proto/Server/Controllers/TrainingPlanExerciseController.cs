namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class TrainingPlanExerciseController
        : AbstractRelationController<ExerciseSet,
          TrainingPlan,
          TrainingPlanExerciseSets>
    {
        public TrainingPlanExerciseController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger<TrainingPlanExerciseController> logger)
            : base(manager, context, logger)
        {
        }
    }
}
