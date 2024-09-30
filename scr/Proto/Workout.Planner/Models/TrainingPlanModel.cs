namespace Workout.Planner.Models
{
    using DataModel;

    public class TrainingPlanModel : ObservableObject
    {
        private Func<TrainingPlanModel, Task> _editPlanFunc;
        private Func<TrainingPlanModel, Task> _deletePlanFunc;
        private Func<TrainingPlanModel, bool> _canEditPlan;
        private Func<TrainingPlanModel, bool> _canDeletePlan;
        private TrainingPlan _plan;
        private string _name;
        private long _id;

        private TrainingPlanModel()
        {
            EditCommand = new Command(EditAsync, CanEdit);
            DeleteCommand = new Command(DeleteAsync, CanDelete);
        }

        public Command EditCommand { get; }

        public Command DeleteCommand { get; }

        public TrainingPlan Plan
        {
            get => _plan;
            private set => SetProperty(ref _plan, value);
        }

        public long Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public static TrainingPlanModel Import(
            TrainingPlan plan,
            Func<TrainingPlanModel, Task> editPlanAsync,
            Func<TrainingPlanModel, bool> canEditPlan,
            Func<TrainingPlanModel, Task> deletePlanAsync,
            Func<TrainingPlanModel, bool> canDeletePlan)
        {
            return new TrainingPlanModel()
            {
                _plan = plan,
                Name = plan.Name,
                Id = plan.Id,
                _editPlanFunc = editPlanAsync,
                _canEditPlan = canEditPlan,
                _deletePlanFunc = deletePlanAsync,
                _canDeletePlan = canDeletePlan,
            };
        }

        public TrainingPlan Export()
        {
            return new TrainingPlan() { Name = Name, Id = Id, CustomerId = _plan.CustomerId, Units = _plan.Units };
        }

        public void RefreshCommands()
        {
            EditCommand?.ChangeCanExecute();
            DeleteCommand?.ChangeCanExecute();
        }

        private bool CanEdit()
        {
            return _editPlanFunc != null && _canEditPlan != null && _canEditPlan.Invoke(this);
        }

        private async void EditAsync()
        {
            await _editPlanFunc.Invoke(this);
        }

        private bool CanDelete()
        {
            return _deletePlanFunc != null && _canDeletePlan != null && _canDeletePlan.Invoke(this);
        }

        private async void DeleteAsync()
        {
            await _deletePlanFunc.Invoke(this);
        }
    }
}
