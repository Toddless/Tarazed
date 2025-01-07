namespace Workout.Planner.Helper
{
    public static class RouteNames
    {
        public static readonly string TrainingPlan = "Training?";
        public static readonly string Unit = "Unit?";
        public static readonly string Exercise = "Exercise?";
        public static readonly string ForgotPassword = "http://localhost:5084/forgotPassword";
        public static readonly string RecoveryPassword = "http://localhost:5084/resetPassword";
        public static readonly string UpdateCustomer = "UpdateCustomer";
        public static readonly string DeleteCustomer = "DeleteCustomer";
        public static readonly string GetCustomer = "GetCustomer";
        public static readonly string LoginRoute = "login";
        public static readonly string Refresh = "refresh";
        public static readonly string Register = "http://localhost:5084/register";
        public static readonly string BaseURI = "http://localhost:5084/";
        public static readonly string Back = "..";
        public static readonly string HomePage = $"//{nameof(HomePage)}";
        public static readonly string LoginPage = $"//{nameof(LoginPage)}";
        public static readonly string UnitPage = $"/{nameof(UnitPage)}";
        public static readonly string ExercisePage = $"/{nameof(ExercisePage)}";
        public static readonly string EditTrainingPage = $"/{nameof(EditTrainingPage)}";
        public static readonly string EditUnitPage = $"/{nameof(EditUnitPage)}";
        public static readonly string EditExercisePage = $"/{nameof(EditExercisePage)}";
        public static readonly string ExerciseDetailPage = $"/{nameof(ExerciseDetailPage)}";
        public static readonly string SettingsPage = $"/{nameof(SettingsPage)}";
        public static readonly string UserPage = $"/{nameof(UserPage)}";
        public static readonly string AboutPage = $"/{nameof(AboutPage)}";
        public static readonly string PasswordRecoveryPage = $"//{nameof(PasswordRecoveryPage)}";
        public static readonly string RegisterUserPage = $"//{nameof(RegisterUserPage)}";

        public static string GetRouteName(string name)
        {
            return name switch
            {
                nameof(TrainingPlan) => TrainingPlan,
                nameof(Unit) => Unit,
                nameof(Exercise) => Exercise,
                _ => string.Empty,
            };
        }
    }
}
