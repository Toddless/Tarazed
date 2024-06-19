namespace Server.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using DataAccessLayer;
    using DataModel;
    using Microsoft.AspNetCore.Mvc;
    using Server.Extensions;
    using Server.Filters;

    [ApiController]
    [Route("[controller]")]
    public class ExerciseController : Controller
    {
        private readonly IDatabaseContext? _context;
        private readonly ILogger? _logger;

        public ExerciseController(IDatabaseContext context, ILogger<ExerciseController> logger)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(logger);
            _context = context;
            _logger = logger;
        }

        [HttpPut("{name}, {description}")]
        public async Task<Exercise?> CreateExerciseAsync(string? name, string? description)
        {
            var context = _context.CheckContext();

            var exercise = new Exercise
            {
                Reps = 1,
                Set = 1,
                Name = name ?? string.Empty,
                Weight = 1,
                Description = description ?? string.Empty,
            };

            bool isValid = IValidateObjectExtension.ValidateObject(exercise, out List<ValidationResult> result);
            if (!isValid)
            {
                result.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            context.ExercisePlans.Add(exercise);
            await context.SaveChangesAsync();
            return exercise;
        }

        [HttpPost]
        public async Task<Exercise> UpdateExerciseAsync(Exercise exercise)
        {
            var context = _context.CheckContext();

            if (exercise == null || exercise.Id == 0L)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Exercise_NotFound));
            }

            bool isValid = IValidateObjectExtension.ValidateObject(exercise, out List<ValidationResult> results);
            if (!isValid)
            {
                results.ForEach(x => _logger?.LogDebug(x.ErrorMessage));
                throw new ServerException(nameof(DataModel.Resources.Errors.InvalidRequest));
            }

            context.ExercisePlans.Update(exercise!);
            var changedCount = await context.SaveChangesAsync();
            if (changedCount != 1)
            {
                throw new ServerException(nameof(DataModel.Resources.Errors.Exercise_NotFound));
            }

            return exercise;
        }
    }
}
