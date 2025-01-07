namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// App settings. Visible only on Home page.
    /// </summary>
    public partial class SettingsPage : BaseView
    {
        public SettingsPage(SettingsPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
