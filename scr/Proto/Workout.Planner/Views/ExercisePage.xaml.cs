namespace Workout.Planner.Views
{
    using Workout.Planner.ViewModels;

    public partial class ExercisePage : BaseView
    {
        public ExercisePage(ExercisePageViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
