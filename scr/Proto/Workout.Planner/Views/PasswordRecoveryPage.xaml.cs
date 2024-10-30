namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page to recovery password. Navigation from login page.
    /// </summary>
    public partial class PasswordRecoveryPage : BaseView
    {
        public PasswordRecoveryPage(PasswordRecoveryPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
