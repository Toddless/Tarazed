namespace Workout.Planner.Models
{
    using DataModel;

    public class ExerciseModel : ObservableObject
    {
        private Func<ExerciseModel, Task> _editExerciseFunc;
        private Func<ExerciseModel, Task> _deleteExerciseFunc;
        private Func<ExerciseModel, bool> _canEditExercise;
        private Func<ExerciseModel, bool> _canDeleteExercise;
        private Exercise _exercise;
        private long _id;
        private string _exerciseName;

        public ExerciseModel()
        {
            EditCommand = new Command(EditAsync, CanEdit);
            DeleteCommand = new Command(DeleteAsync, CanDelete);
        }

        public Command EditCommand { get; set; }

        public Command DeleteCommand { get; set; }

        public Exercise Exercise
        {
            get => _exercise;
            private set => SetProperty(ref _exercise, value);
        }

        public string ExerciseName
        {
            get => _exerciseName;
            set => SetProperty(ref _exerciseName, value);
        }

        public long Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public static IEnumerable<ExerciseModel> Import(
          IEnumerable<Exercise>? exercises,
          Func<ExerciseModel, Task> editExerciseFunc,
          Func<ExerciseModel, bool> canEditExercise,
          Func<ExerciseModel, Task> deleteExerciseFunc,
          Func<ExerciseModel, bool> canDeleteExercise)
        {
            if (exercises != null)
            {
                foreach (var item in exercises)
                {
                    yield return new ExerciseModel()
                    {
                        Exercise = item,
                        ExerciseName = item.Name,
                        Id = item.Id,
                        _editExerciseFunc = editExerciseFunc,
                        _canEditExercise = canEditExercise,
                        _deleteExerciseFunc = deleteExerciseFunc,
                        _canDeleteExercise = canDeleteExercise,
                    };
                }
            }
        }

        public void RefreshCommands()
        {
            EditCommand?.ChangeCanExecute();
            DeleteCommand?.ChangeCanExecute();
        }

        private bool CanEdit()
        {
            return _editExerciseFunc != null && _canEditExercise != null && _canEditExercise.Invoke(this);
        }

        private bool CanDelete()
        {
            return _editExerciseFunc != null && _canEditExercise != null && _canDeleteExercise.Invoke(this);
        }

        private async void EditAsync()
        {
            await _editExerciseFunc.Invoke(this);
        }

        private async void DeleteAsync()
        {
            await _deleteExerciseFunc.Invoke(this);
        }
    }
}
