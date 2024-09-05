namespace Workout.Planner.Services
{
    using DataModel;

    public interface ILoginService
    {
        Task LoginAsync(UserRequest user);

        Task RegisterAsync(UserRequest user);
    }
}
