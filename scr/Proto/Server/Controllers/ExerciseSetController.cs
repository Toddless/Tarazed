namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class ExerciseSetController
        : AbstractRelationController<ExerciseSet,
          Exercise,
          ExerciseSetExercise>
    {
        public ExerciseSetController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger<ExerciseSetController> logger)
            : base(manager, context, logger)
        {
        }
    }
}
