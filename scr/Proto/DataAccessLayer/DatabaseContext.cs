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

        public DbSet<Unit> Units { get; set; }

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

            modelBuilder.Entity<TrainingPlan>()
                .HasMany(o => o.Units)
                .WithOne()
                .HasForeignKey(e => e.TrainingPlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Unit>()
                .HasMany(x => x.Exercises)
                .WithOne()
                .HasForeignKey(o => o.UnitId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}
