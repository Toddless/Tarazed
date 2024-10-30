namespace Workout.Planner.Models
{
    using DataModel;

    public class ExerciseModel : ObservableObject
    {
        private Exercise? _exercise;
        private string? _description;
        private string? _name;
        private long _id;

        public ExerciseModel()
        {
        }

        public Exercise? Exercise
        {
            get => _exercise;
            private set => SetProperty(ref _exercise, value);
        }

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public long Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        public string? Description
        {
            get => _description;
            set
            {
                SetProperty(ref _description, value);
            }
        }

        public static IEnumerable<ExerciseModel> Import(IEnumerable<Exercise>? exercises)
        {
            if (exercises != null)
            {
                foreach (var item in exercises)
                {
                    yield return new ExerciseModel()
                    {
                        Exercise = item,
                        Name = item.Name,
                        Description = item.Description,
                        Id = item.Id,
                    };
                }
            }
        }

        public void RefreshCommands()
        {
        }
    }
}
