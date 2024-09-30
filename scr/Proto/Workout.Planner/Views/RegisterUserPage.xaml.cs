namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    public partial class RegisterUserPage : BaseView
    {
        public RegisterUserPage(RegisterUserPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
