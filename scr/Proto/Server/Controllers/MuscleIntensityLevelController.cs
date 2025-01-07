namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;

    public class MuscleIntensityLevelController : AbstractBaseController<MuscleIntensityLevel>
    {
        public MuscleIntensityLevelController(
            UserManager<ApplicationUser> manager,
            IDatabaseContext context,
            ILogger<MuscleIntensityLevelController> logger)
            : base(manager, context, logger)
        {
        }

        protected override IQueryable<MuscleIntensityLevel> AddIncludes(IQueryable<MuscleIntensityLevel> query)
        {
            return query;
        }
    }
}
