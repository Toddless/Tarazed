namespace Workout.Planner.Models
{
    using DataModel;

    public class UnitModel : ObservableObject
    {
        private Func<UnitModel, Task>? _editUnitFunc;
        private Func<UnitModel, Task>? _deleteUnitFunc;
        private Func<UnitModel, bool>? _canEditUnit;
        private Func<UnitModel, bool>? _canDeleteUnit;
        private string? _unitName;
        private Workout? _unit;
        private long _id;

        private UnitModel()
        {
            EditCommand = new Command(EditAsync, CanEdit);
            DeleteCommand = new Command(DeleteAsync, CanDelete);
        }

        public Command EditCommand { get; }

        public Command DeleteCommand { get; }

        public Workout? Unit
        {
            get => _unit;
            private set => SetProperty(ref _unit, value);
        }

        public long Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        public string? UnitName
        {
            get => _unitName;
            set => SetProperty(ref _unitName, value);
        }

        public static IEnumerable<UnitModel> Import(
            IEnumerable<Workout>? units,
            Func<UnitModel, Task> editUnitAsync,
            Func<UnitModel, bool> canEditUnit,
            Func<UnitModel, Task> deleteUnitAsync,
            Func<UnitModel, bool> canDeleteUnit)
        {
            if (units != null)
            {
                foreach (var item in units)
                {
                    yield return new UnitModel()
                    {
                        Unit = item,
                        UnitName = item.Name,
                        Id = item.Id,
                        _editUnitFunc = editUnitAsync,
                        _canEditUnit = canEditUnit,
                        _deleteUnitFunc = deleteUnitAsync,
                        _canDeleteUnit = canDeleteUnit,
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
            return _editUnitFunc != null && _canEditUnit != null && _canEditUnit.Invoke(this);
        }

        private async void EditAsync()
        {
            await _editUnitFunc?.Invoke(this) !;
        }

        private bool CanDelete()
        {
            return _editUnitFunc != null && _canEditUnit != null && _canDeleteUnit!.Invoke(this);
        }

        private async void DeleteAsync()
        {
            await _deleteUnitFunc?.Invoke(this) !;
        }
    }
}
