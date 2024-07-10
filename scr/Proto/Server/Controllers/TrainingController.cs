namespace Server.Controllers
{
    using System.Threading.Tasks;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Server.Extensions;
    using Server.Filters;

    public class TrainingController
        : AbstractRelationController<TrainingPlan,
          ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _manager;

        public TrainingController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<TrainingController> logger)
            : base(context, logger)
        {
            _manager = manager;
        }

        [HttpPut]
        public override async Task<TrainingPlan?> CreateAsync(TrainingPlan item)
        {
            try
            {
                var currentUser = await _manager.GetUserAsync(User);
                if (currentUser == null)
                {
                    throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
                }

                var customer = new Customer { Ids = currentUser.Ids };
                item.CustomerId = customer.Ids;
                return await base.CreateAsync(item);
            }
            catch (ServerException)
            {
                throw;
            }
        }
    }
}
