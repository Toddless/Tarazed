namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;

    public class ExerciseController
        : AbstractBaseController<Exercise>
    {
        public ExerciseController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<ExerciseController> logger)
            : base(manager, context, logger)
        {
        }
    }
}
