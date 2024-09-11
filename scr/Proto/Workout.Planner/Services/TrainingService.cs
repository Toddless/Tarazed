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
            route = CreateRouteExtensions.CreateGetStringRoute(ids, route, additionalData);
            return await _restAPIService.GetAsync<TrainingPlan>(route, token).ConfigureAwait(false);
        }

        public async Task<TrainingPlan> UpdateTrainingAsync(TrainingPlan plan, CancellationToken token)
        {
            string route = RouteNames.Training;
            route = CreateRouteExtensions.ObjToQuery(plan, route);
            return await _restAPIService.PostAsync<TrainingPlan, TrainingPlan>(route, plan, token).ConfigureAwait(false);
        }

        public async Task<TrainingPlan> CreateTrainingAsync(TrainingPlan plan, CancellationToken token)
        {
            string route = RouteNames.Training;
            route = CreateRouteExtensions.ObjToQuery(plan, route);
            return await _restAPIService.PutAsync<TrainingPlan, TrainingPlan>(route, plan, token).ConfigureAwait(false);
        }

        public async Task<bool> DeleteTrainingAsync(IEnumerable<long>? ids, CancellationToken token)
        {
            string route = RouteNames.Training;
            route = CreateRouteExtensions.CreateDeleteStringRoute(ids, route);
            await _restAPIService.DeleteAsync(route, token).ConfigureAwait(false);
            return true;
        }
    }
}
