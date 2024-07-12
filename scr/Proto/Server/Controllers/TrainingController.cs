namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    public class TrainingController
        : AbstractBaseController<TrainingPlan>
    {
        public TrainingController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<TrainingController> logger)
            : base(manager, context, logger)
        {
        }
    }
}
