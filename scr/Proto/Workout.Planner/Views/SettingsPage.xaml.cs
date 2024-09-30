namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    public partial class SettingsPage : BaseView
    {
        public SettingsPage(SettingsPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
