namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page with user units.
    /// </summary>
    public partial class UnitPage : BaseView
    {
        public UnitPage(UnitPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
