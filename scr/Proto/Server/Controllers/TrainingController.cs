namespace Server.Controllers
{
    using System.Linq;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
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

        protected override IQueryable<TrainingPlan> AddIncludes(IQueryable<TrainingPlan> query)
        {
            return query.Include(x => x.Units);
        }
    }
}
