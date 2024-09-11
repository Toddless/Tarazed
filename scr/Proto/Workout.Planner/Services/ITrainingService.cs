namespace Workout.Planner.Services
{
    using System.Threading;
    using DataModel;

    public interface ITrainingService
    {
        Task<TrainingPlan> CreateTrainingAsync(TrainingPlan plan, CancellationToken token);

        Task<bool> DeleteTrainingAsync(IEnumerable<long>? ids, CancellationToken token);

        Task<IEnumerable<TrainingPlan>> GetTrainingAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null);

        Task<TrainingPlan> UpdateTrainingAsync(TrainingPlan plan, CancellationToken token);
    }
}
