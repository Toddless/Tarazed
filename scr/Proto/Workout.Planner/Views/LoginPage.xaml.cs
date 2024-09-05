namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    public partial class LoginPage : BaseView
    {
        public LoginPage(LoginPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
