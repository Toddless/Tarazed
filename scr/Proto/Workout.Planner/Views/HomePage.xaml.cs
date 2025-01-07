namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Start page. Contains Training plans.
    /// </summary>
    public partial class HomePage : BaseView
    {
        public HomePage(HomePageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
