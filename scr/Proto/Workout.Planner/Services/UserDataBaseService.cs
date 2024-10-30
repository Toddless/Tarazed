namespace Workout.Planner.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;

    public abstract class UserDataBaseService<TU>
        where TU : class
    {
        private readonly IRestApiService _restApiService;

        public UserDataBaseService([FromKeyedServices("AuthRestAPI")] IRestApiService restApiService)
        {
            ArgumentNullException.ThrowIfNull(restApiService);
            _restApiService = restApiService;
        }

        public virtual Task<IEnumerable<TU>> GetDataAsync(bool additionalData, CancellationToken token, IEnumerable<long>? ids = null)
        {
            string route = RouteNames.GetRouteName(typeof(TU).Name);
            route = CreateRouteExtensions.CreateGetStringRoute(ids, route, additionalData);
            return _restApiService.GetAsync<TU>(route, token);
        }

        public virtual Task<TU> UpdataDataAsync(TU item, CancellationToken token)
        {
            string route = RouteNames.GetRouteName(typeof(TU).Name);
            route = CreateRouteExtensions.ObjToQuery(item, route);
            return _restApiService.PostAsync<TU, TU>(route, item, token);
        }

        public virtual Task<TU> CreateDataAsync(TU item, CancellationToken token)
        {
            string route = RouteNames.GetRouteName(typeof(TU).Name);
            route = CreateRouteExtensions.ObjToQuery(item, route);
            return _restApiService.PutAsync<TU, TU>(route, item, token);
        }

        public virtual bool DeleteDataAsync(IEnumerable<long>? ids, CancellationToken token)
        {
            string route = RouteNames.GetRouteName(typeof(TU).Name);
            route = CreateRouteExtensions.CreateDeleteStringRoute(ids, route);
            _restApiService.DeleteAsync(route, token);
            return true;
        }
    }
}
