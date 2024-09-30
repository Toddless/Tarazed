namespace Workout.Planner.Services.Contracts
{
    using DataModel;
    using Workout.Planner.Models;

    public interface ILoginService
    {
        Task LoginAsync(UserRequest user);

        Task RegisterAsync(UserRequest user);

        Task RecoverUserPasswordAsync(PasswordRecoveryModel recoveryPassword, bool forgotPassword);
    }
}
