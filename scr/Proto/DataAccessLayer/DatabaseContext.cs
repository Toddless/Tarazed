namespace DataAccessLayer
{
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext
        : IdentityDbContext<ApplicationUser, IdentityRole, string>, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<Workout> Units { get; set; }

        public DbSet<TrainingPlan> TrainingPlans { get; set; }

        public DbSet<MuscleIntensityLevel> MuscleIntensityLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Customer>();

            modelBuilder.Entity<TrainingPlan>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasPrincipalKey(x => x.Id)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingPlan>()
                .HasMany(x => x.Workout)
                .WithOne()
                .HasForeignKey(x => x.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Workout>()
                .HasMany(x => x.Exercises)
                .WithOne()
                .HasForeignKey(x => x.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Exercise>()
                .HasMany(x => x.MuscleIntensityLevelId)
                .WithOne()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
