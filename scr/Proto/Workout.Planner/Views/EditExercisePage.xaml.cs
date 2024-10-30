namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page to change Exercises.
    /// </summary>
    public partial class EditExercisePage : BaseView
    {
        public EditExercisePage(EditExercisePageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
