namespace DataAccessLayer
{
    using System.Threading.Tasks;
    using DataModel;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    public interface IDatabaseContext : IDisposable
    {
        DbSet<TU> Set<TU>()
            where TU : class;

        DatabaseFacade Database { get; }

        DbSet<CustomerTrainingPlan> CustomerTrainingPlans { get; set; }

        DbSet<Exercise> Exercises { get; set; }

        DbSet<ExerciseSet> ExerciseSets { get; set; }

        DbSet<ExerciseSetExercise> ExerciseSetsExercise { get; set; }

        DbSet<TrainingPlanExerciseSets> TrainingPlanExerciseSets { get; set; }

        DbSet<TrainingPlan> TrainingPlans { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
