namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    public partial class HomePage : BaseView
    {
        public HomePage(HomePageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
