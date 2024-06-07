namespace DataAccessLayer
{
    using DataModel;
    using Microsoft.EntityFrameworkCore;

    public interface IDatabaseContext : IDisposable
    {
        DbSet<Customer> Customers { get; set; }

        DbSet<CustomerTrainingPlan> CustomerTrainingPlans { get; set; }

        DbSet<Exercise> ExercisePlans { get; set; }

        DbSet<ExerciseSet> ExerciseSets { get; set; }

        DbSet<ExerciseSetExercise> ExerciseSetsExercise { get; set; }

        DbSet<TrainingPlanExerciseSets> TrainingPlanExerciseSets { get; set; }

        DbSet<TrainingPlan> TrainingPlans { get; set; }
    }
}
