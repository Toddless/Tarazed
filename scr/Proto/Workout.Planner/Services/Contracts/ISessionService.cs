namespace Workout.Planner.Services.Contracts
{
    using DataModel;

    public interface ISessionService
    {
        void Initialize(LoginService loginService);

        Task SetTokenAsync(AccessTokenResponse token);

        Task EnsureAccessTokenNotExpiredAsync();

        Task UserLogoutAsync();
    }
}
