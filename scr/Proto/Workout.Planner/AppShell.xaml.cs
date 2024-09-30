namespace Workout.Planner
{
    using Workout.Planner.Extensions;
    using Workout.Planner.ViewModels;
    using Workout.Planner.Views;

    public partial class AppShell : Shell
    {
        private readonly AppShellViewModel _viewModel;

        public AppShell(AppShellViewModel viewModel)
        {
            InitializeComponent();
            Routing.RegisterRoute(RouteNames.HomePage, typeof(HomePage));
            Routing.RegisterRoute(RouteNames.LoginPage, typeof(LoginPage));
            Routing.RegisterRoute(RouteNames.SettingsPage, typeof(SettingsPage));
            Routing.RegisterRoute(RouteNames.EditTrainingPage, typeof(EditTrainingPage));
            Routing.RegisterRoute(RouteNames.SettingsPage, typeof(SettingsPage));
            Routing.RegisterRoute(RouteNames.UserPage, typeof(UserPage));
            Routing.RegisterRoute(RouteNames.PasswordRecoveryPage, typeof(PasswordRecoveryPage));
            Routing.RegisterRoute(RouteNames.RegisterUserPage, typeof(RegisterUserPage));
            Routing.RegisterRoute(RouteNames.UnitPage, typeof(UnitPage));
            Routing.RegisterRoute(RouteNames.AboutPage, typeof(AboutPage));
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }
}
