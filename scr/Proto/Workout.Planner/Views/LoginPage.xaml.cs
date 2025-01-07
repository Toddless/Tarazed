namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page for loggin.
    /// </summary>
    public partial class LoginPage : BaseView
    {
        public LoginPage(LoginPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
