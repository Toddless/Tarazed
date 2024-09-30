namespace Workout.Planner.Services
{
    using DataModel;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services.Contracts;

    public class UserService : IUserService
    {
        private IRestApiService _restAPIService;

        public UserService([FromKeyedServices("AuthRestAPI")] IRestApiService restAPIService)
        {
            _restAPIService = restAPIService;
        }

        public async Task<Customer> GetCurrentUser(CancellationToken token)
        {
            string route = RouteNames.GetCustomer;
            return await _restAPIService.GetUserAsync<Customer>(route, token);
        }

        public async Task<Customer> UpdateCustomerAsync(CancellationToken token, Customer customer)
        {
            string route = RouteNames.UpdateCustomer;
            return await _restAPIService.PutAsync<Customer, Customer>(route, customer, token);
        }

        public async Task<bool> DeleteUserAsync(CancellationToken token)
        {
            string route = RouteNames.DeleteCustomer;
            return await _restAPIService.DeleteAsync(route, token);
        }
    }
}
