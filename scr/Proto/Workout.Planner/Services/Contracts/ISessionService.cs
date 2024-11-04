namespace Workout.Planner.Services.Contracts
{
    using DataModel;

    public interface ISessionService
    {
        void Initialize(LoginService loginService);

        Task SetTokenAsync(TokenHandlingModel token);

        Task EnsureAccessTokenNotExpiredAsync(CancellationToken token);

        Task UserLogoutAsync();
    }
}
