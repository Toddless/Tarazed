namespace Workout.Planner.ViewModels
{
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class EditExercisePageViewModel : EditViewModelBase<Exercise>
    {
        public EditExercisePageViewModel(
            INavigationService navigationService,
            ILogger<EditExercisePageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IExerciseService service)
            : base(navigationService, logger, dispatcher, sessionService, service)
        {
        }

        protected override string EntityName => AppStrings.ExerciseEntityName;

        protected override Exercise GetUpdateEntity()
        {
            if (RelatedId != 0)
            {
                _entity!.UnitId = (long)RelatedId!;
                _entity.Weight = 1;
                _entity.Reps = 1;
                _entity.Set = 1;
            }

            _entity!.Name = this.Name!;
            return _entity;
        }

        protected override void LoadOnUI(Exercise exercise)
        {
            _entity!.Name = exercise.Name;
        }

        protected override bool CanSaveChanges()
        {
            return base.CanSaveChanges() && _entity != null && Name != _entity.Name;
        }
    }
}
