﻿namespace Server.Controllers
{
    using System.Linq;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public class UnitController
        : AbstractBaseController<Workout>
    {
        public UnitController(
            IDatabaseContext context,
            UserManager<ApplicationUser> manager,
            ILogger<UnitController> logger)
            : base(manager, context, logger)
        {
        }

        protected override IQueryable<Workout> AddIncludes(IQueryable<Workout> query)
        {
            return query.Where(x => x.Exercises != null).Include(x => x.Exercises!).ThenInclude(x => x.MuscleIntensityLevelId);
        }
    }
}
