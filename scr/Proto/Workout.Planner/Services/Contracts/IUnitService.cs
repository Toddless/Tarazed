namespace Workout.Planner.Services.Contracts
{
    using DataModel;

    public interface IUnitService
    {
        Task<Unit> CreateDataAsync(Unit item, CancellationToken token);

        Task<bool> DeleteDataAsync(IEnumerable<long>? ids, CancellationToken token);

        Task<IEnumerable<Unit>> GetDataAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null);

        Task<Unit> UpdataDataAsync(Unit item, CancellationToken token);
    }
}
