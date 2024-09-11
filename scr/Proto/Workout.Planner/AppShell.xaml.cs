namespace Workout.Planner
{
    using Workout.Planner.Extensions;
    using Workout.Planner.Views;

    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(RouteNames.HomePage, typeof(HomePage));
            Routing.RegisterRoute(RouteNames.LoginPage, typeof(LoginPage));
            Routing.RegisterRoute(RouteNames.EditTrainingPage, typeof(EditTrainingPage));
        }
    }
}
