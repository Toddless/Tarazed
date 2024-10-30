namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page to edit user profile.
    /// </summary>
    public partial class UserPage : BaseView
    {
        public UserPage(UserPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
