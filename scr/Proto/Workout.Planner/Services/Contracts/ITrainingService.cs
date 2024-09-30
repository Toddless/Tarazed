namespace Workout.Planner.Services.Contracts
{
    using DataModel;

    public interface ITrainingService
    {
        Task<TrainingPlan> CreateDataAsync(TrainingPlan item, CancellationToken token);

        Task<bool> DeleteDataAsync(IEnumerable<long>? ids, CancellationToken token);

        Task<IEnumerable<TrainingPlan>> GetDataAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null);

        Task<TrainingPlan> UpdataDataAsync(TrainingPlan item, CancellationToken token);
    }
}
