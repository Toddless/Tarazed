 namespace Server.Controllers
{
    using System.Threading.Tasks;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Server.Extensions;
    using Server.Filters;

    [ApiController]
    [Route("[controller]")]
    public class ExerciseController
        : AbstractRelationController<Exercise,
            ExerciseSet>
    {
        private readonly IDatabaseContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public ExerciseController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<ExerciseController> logger)
            : base(context, logger)
        {
            _manager = manager;
            _context = context;
        }

        public override async Task<Exercise?> CreateAsync(Exercise item)
        {
            var context = _context.CheckContext();
            try
            {
                var customer = await _manager.GetUserAsync(User);
                if (customer == null)
                {
                    throw new ServerException("Error");
                }

                var exerciseSet = await context.ExerciseSets.FirstOrDefaultAsync(o => o.Ids == item.ExerciseSetId);
                if (exerciseSet == null || exerciseSet.Ids == 0)
                {
                    throw new ServerException("Error");
                }

                item.ExerciseSet = exerciseSet;
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
