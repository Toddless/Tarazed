namespace Workout.Planner
{
    using Microsoft.Maui;
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Initialize MainPage.
    /// </summary>
    public partial class App : Application
    {
        private readonly AppShellViewModel? _viewModel;

        public App(AppShellViewModel appShellViewModel)
        {
            ArgumentNullException.ThrowIfNull(appShellViewModel);
            InitializeComponent();
            _viewModel = appShellViewModel;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var windowWithHeightWidth = new Window(new AppShell(_viewModel!)) { MinimumWidth = 750, MinimumHeight = 650 };
            return windowWithHeightWidth;
        }
    }
}
