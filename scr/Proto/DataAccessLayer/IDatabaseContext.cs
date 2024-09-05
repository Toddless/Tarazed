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

        DbSet<Exercise> Exercises { get; set; }

        DbSet<Unit> Units { get; set; }

        DbSet<TrainingPlan> TrainingPlans { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
