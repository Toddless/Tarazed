namespace DataAccessLayer
{
    using DataModel;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerTrainingPlan> CustomerTrainingPlans { get; set; }

        public DbSet<Exercise> ExercisePlans { get; set; }

        public DbSet<ExerciseSet> ExerciseSets { get; set; }

        public DbSet<ExerciseSetExercise> ExerciseSetsExercise { get; set; }

        public DbSet<TrainingPlan> TrainingPlans { get; set; }

        public DbSet<TrainingPlanExerciseSets> TrainingPlanExerciseSets { get; set; }
    }
}
