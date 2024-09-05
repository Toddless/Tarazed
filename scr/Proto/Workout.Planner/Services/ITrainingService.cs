namespace Workout.Planner.Services
{
    using System.Threading;
    using DataModel;

    public interface ITrainingService
    {
        Task<IEnumerable<TrainingPlan>> GetTrainingAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null);
    }
}
