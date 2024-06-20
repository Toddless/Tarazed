namespace Server.Controllers
{
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    public class ExerciseController : ControllerBase<Exercise>
    {
        public ExerciseController(IDatabaseContext context, ILogger<ExerciseController> logger)
            : base(context, logger)
        {
        }
    }
}
