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

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<ExerciseSet> ExerciseSets { get; set; }

        public DbSet<ExerciseSetExercise> ExerciseSetsExercise { get; set; }

        public DbSet<TrainingPlan> TrainingPlans { get; set; }

        public DbSet<TrainingPlanExerciseSets> TrainingPlanExerciseSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerTrainingPlan>()
                .HasOne(x => x.TrainingPlan)
                .WithMany()
                .HasForeignKey(x => x.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerTrainingPlan>()
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingPlanExerciseSets>()
                .HasOne(x => x.TrainingPlan)
                .WithMany()
                .HasForeignKey(x => x.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingPlanExerciseSets>()
                .HasOne(x => x.ExerciseSet)
                .WithMany()
                .HasForeignKey(x => x.ExerciseSetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExerciseSetExercise>()
                .HasOne(x => x.ExerciseSet)
                .WithMany()
                .HasForeignKey(x => x.ExerciseSetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ExerciseSetExercise>()
                .HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
