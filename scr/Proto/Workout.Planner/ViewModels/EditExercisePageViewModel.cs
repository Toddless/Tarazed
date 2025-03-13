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
                Entity!.UnitId = (long)RelatedId!;
                Entity.Weight = 1;
                Entity.Reps = 1;
                Entity.Set = 1;
            }

            Entity!.Name = this.Name!;
            return Entity;
        }

        protected override void LoadOnUI(Exercise exercise)
        {
            Entity!.Name = exercise.Name;
        }

        protected override bool CanSaveChanges()
        {
            return base.CanSaveChanges() && Entity != null && Name != Entity.Name;
        }
    }
}
