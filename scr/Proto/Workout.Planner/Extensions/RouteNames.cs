namespace Workout.Planner.Extensions
{
    public static class RouteNames
    {
        public static readonly string Training = "Training?";
        public static readonly string Units = "Unit?";
        public static readonly string Exercise = "Exercise?";
        public static readonly string UpdateCustomer = "UpdateCustomer";
        public static readonly string DeleteCustomer = "DeleteCustomer";
        public static readonly string GetCustomer = "GetCustomer";
        public static readonly string LoginControllerRoute = "login";
        public static readonly string Refresh = "refresh";
        public static readonly string Register = "register";
        public static readonly string BaseURI = "http://localhost:5084/";
        public static readonly string Back = "..";
        public static readonly string HomePage = $"//{nameof(HomePage)}";
        public static readonly string LoginPage = $"//{nameof(LoginPage)}";
        public static readonly string UnitPage = $"/{nameof(UnitPage)}";
        public static readonly string PopupPage = $"//{nameof(PopupPage)}";
        public static readonly string EditTrainingPage = $"//{nameof(EditTrainingPage)}";
    }
}
