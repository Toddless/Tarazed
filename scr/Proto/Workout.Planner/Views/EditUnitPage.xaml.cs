namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page to edit Units.
    /// </summary>
    public partial class EditUnitPage : BaseView
    {
        public EditUnitPage(EditUnitPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
