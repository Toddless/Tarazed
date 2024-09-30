namespace Workout.Planner.Services.Contracts
{
    using DataModel;

    public interface IUserService
    {
        Task<Customer> GetCurrentUser(CancellationToken token);

        Task<Customer> UpdateCustomerAsync(CancellationToken token, Customer customer);

        Task<bool> DeleteUserAsync(CancellationToken token);
    }
}
