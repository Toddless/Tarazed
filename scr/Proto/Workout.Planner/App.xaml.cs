namespace Workout.Planner
{
    using Microsoft.Maui;
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

        /// <summary>
        /// Create new Window.
        /// </summary>
        /// <param name="activationState">To be added.</param>
        /// <returns> Return new window with Minimum Width and Height. X and Y define initial window position.</returns>
        protected override Window CreateWindow(IActivationState? activationState) =>
            new(MainPage!) { MinimumWidth = 600, MinimumHeight = 500, X = 200, Y = 200};
    }
}
