namespace Workout.Planner.Models
{
    using DataModel;

    public class ExerciseModel : ObservableObject
    {
        private Exercise _exercise;
        private string? _description;
        private string _name;
        private long _id;

        public ExerciseModel()
        {
        }

        public Command EditCommand { get; set; }

        public Command DeleteCommand { get; set; }

        public Exercise Exercise
        {
            get => _exercise;
            private set => SetProperty(ref _exercise, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public long Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string? Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
            }
        }

        public static ExerciseModel Import(IEnumerable<Exercise>? exercises)
        {
            if (exercises == null)
            {
                throw new Exception();
            }

            var exercise = exercises.First();

            return new ExerciseModel()
            {
                Exercise = exercise,
                Id = exercise.Id,
                Description = exercise.Description,
                Name = exercise.Name,
            };
        }

        public void RefreshCommands()
        {
            EditCommand?.ChangeCanExecute();
            DeleteCommand?.ChangeCanExecute();
        }
    }
}
