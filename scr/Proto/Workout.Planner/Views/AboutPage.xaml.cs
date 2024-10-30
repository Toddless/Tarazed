namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// About page.
    /// </summary>
    public partial class AboutPage : BaseView
    {
        public AboutPage(AboutPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
