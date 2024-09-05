namespace Workout.Planner.Services
{
    using DataModel;
    using Workout.Planner.Extensions;

    public class TrainingService : ITrainingService
    {
        private IRestApiService _restAPIService;

        public TrainingService([FromKeyedServices("AuthRestAPI")] IRestApiService restAPIService)
        {
            _restAPIService = restAPIService;
        }

        public async Task<IEnumerable<TrainingPlan>> GetTrainingAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null)
        {
            string route = RouteNames.Training;
            route = CreateRouteExtensions.CreateStringRoute(ids, route, additionalData);
            return await _restAPIService.GetAsync<TrainingPlan>(route, token).ConfigureAwait(false);
        }
    }
}
