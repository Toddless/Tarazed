namespace Workout.Planner
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Initialize MainPage.
    /// </summary>
    public partial class App : Application
    {
        public App(AppShellViewModel appShellViewModel)
        {
            InitializeComponent();
            MainPage = new AppShell(appShellViewModel);
        }
    }
}
