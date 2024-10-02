namespace Workout.Planner.Services.Contracts
{
    using DataModel;

    public interface IService<TEntity>
        where TEntity : class, IHaveName, new()
    {
        Task<TEntity> CreateDataAsync(TEntity item, CancellationToken token);

        Task<bool> DeleteDataAsync(IEnumerable<long>? ids, CancellationToken token);

        Task<IEnumerable<TEntity>> GetDataAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null);

        Task<TEntity> UpdataDataAsync(TEntity item, CancellationToken token);
    }
}
