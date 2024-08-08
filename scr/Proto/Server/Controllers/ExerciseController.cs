namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;

    public class ExerciseController
        : AbstractBaseController<Exercise>
    {
        public ExerciseController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger<ExerciseController> logger)
            : base(manager, context, logger)
        {
        }
    }
}
