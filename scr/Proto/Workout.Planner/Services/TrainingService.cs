namespace Workout.Planner.Services
{
    using DataModel;
    using Microsoft.Extensions.DependencyInjection;
    using Workout.Planner.Services.Contracts;

    public class TrainingService : UserDataBaseService<TrainingPlan>, ITrainingService
    {
        public TrainingService([FromKeyedServices("AuthRestAPI")] IRestApiService restApiService)
            : base(restApiService)
        {
        }
    }
}
