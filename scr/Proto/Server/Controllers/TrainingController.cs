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

        //[HttpGet]
        //public async Task<TrainingPlan?> GetAsync(long id, bool isChildNeeded)
        //{
        //    try
        //    {
        //        var context = _context.CheckContext();
        //        var currentUser = await _manager.GetUserAsync(User);

        //        if (currentUser == null)
        //        {
        //            throw new ServerException(DataModel.Resources.Errors.InvalidRequest);
        //        }

        //        if (isChildNeeded)
        //        {
        //            return await context.Set<TrainingPlan>()
        //                .Include(x => x.Units)
        //                .Where(o => o.CustomerId == currentUser.Id && o.PrimaryId == id)
        //                .AsNoTracking()
        //                .SingleOrDefaultAsync();
        //        }
        //        else
        //        {
        //            return
        //        }
        //    }
        //    catch (ServerException)
        //    {
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogException(this, ex);
        //        _logger.LogError(ex, $"Unknown Error in {nameof(GetAsync)}.");
        //        throw new InternalServerException(DataModel.Resources.Errors.InternalException);
        //    }
        //}

    }
}
