namespace Workout.Planner.Models
{
    public class PasswordRecoveryModel
    {
        public string? Email { get; set; }

        public string? ResetCode { get; set; }

        public string? NewPassword { get; set; }
    }
}
