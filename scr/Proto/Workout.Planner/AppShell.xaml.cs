namespace Workout.Planner
{
    using Workout.Planner.Helper;
    using Workout.Planner.ViewModels;
    using Workout.Planner.Views;

    public partial class AppShell : Shell
    {
        private readonly AppShellViewModel _viewModel;

        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            Routing.RegisterRoute(RouteNames.PasswordRecoveryPage, typeof(PasswordRecoveryPage));
            Routing.RegisterRoute(RouteNames.RegisterUserPage, typeof(RegisterUserPage));
            Routing.RegisterRoute(RouteNames.EditTrainingPage, typeof(EditTrainingPage));
            Routing.RegisterRoute(RouteNames.EditExercisePage, typeof(EditExercisePage));
            Routing.RegisterRoute(RouteNames.EditUnitPage, typeof(EditUnitPage));
            Routing.RegisterRoute(RouteNames.SettingsPage, typeof(SettingsPage));
            Routing.RegisterRoute(RouteNames.ExercisePage, typeof(ExercisePage));
            Routing.RegisterRoute(RouteNames.LoginPage, typeof(LoginPage));
            Routing.RegisterRoute(RouteNames.AboutPage, typeof(AboutPage));
            Routing.RegisterRoute(RouteNames.HomePage, typeof(HomePage));
            Routing.RegisterRoute(RouteNames.UserPage, typeof(UserPage));
            Routing.RegisterRoute(RouteNames.UnitPage, typeof(UnitPage));
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }
}
