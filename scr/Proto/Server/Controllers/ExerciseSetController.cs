namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class ExerciseSetController
        : AbstractBaseController<ExerciseSet>
    {
        public ExerciseSetController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<ExerciseSetController> logger)
            : base(manager, context, logger)
        {
        }
    }
}
