namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page to edit trainings.
    /// </summary>
    public partial class EditTrainingPage : BaseView
    {
        public EditTrainingPage(EditTrainingPageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
