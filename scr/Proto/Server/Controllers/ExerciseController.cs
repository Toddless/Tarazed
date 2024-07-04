namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ExerciseController : ControllerBase<Exercise>
    {
        public ExerciseController(IDatabaseContext context, UserManager<IdentityUser> manager, ILogger<ExerciseController> logger)
            : base(context, manager, logger)
        {
        }
    }
}
