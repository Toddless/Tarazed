namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page to register new user. Navigation from Login page.
    /// </summary>
    public partial class RegisterUserPage : BaseView
    {
        public RegisterUserPage(RegisterUserPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
