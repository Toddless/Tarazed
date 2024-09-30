namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    public partial class UserPage : BaseView
    {
        public UserPage(UserPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
