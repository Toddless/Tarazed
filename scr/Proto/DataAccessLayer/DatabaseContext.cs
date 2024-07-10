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

        public DbSet<CustomerTrainingPlan> CustomerTrainingPlans { get; set; }

        public DbSet<Exercise> Exercises { get; set; }

        public DbSet<ExerciseSet> ExerciseSets { get; set; }

        public DbSet<ExerciseSetExercise> ExerciseSetsExercise { get; set; }

        public DbSet<TrainingPlan> TrainingPlans { get; set; }

        public DbSet<TrainingPlanExerciseSets> TrainingPlanExerciseSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Customer>();
            #region
            //modelBuilder.Entity<IdentityUserRole<Guid>>()
            //    .HasKey(p => new
            //    {
            //        p.UserId,
            //        p.RoleId,
            //    });

            //modelBuilder.Entity<ApplicationUser>(b =>
            //{
            //    b.HasMany(e => e.Tokens)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(ul => ul.UserId)
            //    .IsRequired();

            //    b.HasMany(e => e.UserRoles)
            //    .WithOne(e => e.User)
            //    .HasForeignKey(ur => ur.UserId)
            //    .IsRequired();
            //});

            //modelBuilder.Entity<ApplicationRole>(b =>
            //{
            //    b.HasMany(e => e.UserRole)
            //    .WithOne(e => e.Role)
            //    .HasForeignKey(ul => ul.RoleId)
            //    .IsRequired();
            //});
            #endregion

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<CustomerTrainingPlan>()
                .HasOne(x => x.TrainingPlan)
                .WithMany()
                .HasForeignKey(x => x.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerTrainingPlan>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasPrincipalKey(x => x.Id)
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
