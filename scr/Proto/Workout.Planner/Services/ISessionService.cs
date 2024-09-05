namespace Workout.Planner.Services
{
    using DataModel;

    public interface ISessionService
    {
        void Initialize(LoginService loginService);

        Task SetTokenAsync(AccessTokenResponse token);

        Task EnsureTokenAsync();
    }
}
