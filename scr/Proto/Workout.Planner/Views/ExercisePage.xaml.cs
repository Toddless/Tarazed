namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    /// <summary>
    /// Page with user exercises.
    /// </summary>
    public partial class ExercisePage : BaseView
    {
        public ExercisePage(ExercisePageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
