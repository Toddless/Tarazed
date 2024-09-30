namespace Workout.Planner
{
    using Workout.Planner.ViewModels;

    public partial class App : Application
    {
        public App(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            MainPage = new AppShell(appShellViewModel);
        }
    }
}
