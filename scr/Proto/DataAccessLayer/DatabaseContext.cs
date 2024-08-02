namespace DataAccessLayer
{
    using DataModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext
        : IdentityDbContext<ApplicationUser, IdentityRole, string>, IDatabaseContext
    {
        /// <exception cref="ArgumentNullException">Throws if null.</exception>
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<ExerciseSet> ExerciseSets { get; set; }

        public DbSet<TrainingPlan> TrainingPlans { get; set; }

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

            modelBuilder.Entity<ExerciseSet>()
                .HasOne(o => o.TrainingPlan)
                .WithMany()
                .HasForeignKey(o => o.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exercise>()
                .HasOne(o => o.ExerciseSet)
                .WithMany()
                .HasForeignKey(o => o.ExerciseSetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
