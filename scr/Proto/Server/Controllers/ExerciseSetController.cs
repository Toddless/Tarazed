namespace Server.Controllers
{
    using System.Threading.Tasks;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Server.Extensions;
    using Server.Filters;

    public class ExerciseSetController
        : AbstractRelationController<ExerciseSet,
          TrainingPlan>
    {
        private readonly IDatabaseContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public ExerciseSetController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<ExerciseSetController> logger)
            : base(context, logger)
        {
            _manager = manager;
            _context = context;
        }

        public override async Task<ExerciseSet?> CreateAsync(ExerciseSet item)
        {
            var context = _context.CheckContext();
            try
            {
                var customer = await _manager.GetUserAsync(User);
                if (customer == null)
                {
                    throw new ServerException("Error");
                }

                var trainingPlan = await context.TrainingPlans.FirstOrDefaultAsync(o => o.Ids == item.TrainingPlanId);
                if (trainingPlan == null || trainingPlan.Ids == 0)
                {
                    throw new ServerException("Error");
                }

                if (trainingPlan.CustomerId != customer.Ids)
                {
                    throw new ServerException("Error");
                }

                item.TrainingPlan = trainingPlan;
                await base.CreateAsync(item);

                return item;
            }
            catch (ServerException)
            {
                throw;
            }
        }
    }
}
