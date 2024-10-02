namespace Workout.Planner.ViewModels
{
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Services.Contracts;

    public class EditTrainingPageViewModel : EditViewModelBase<TrainingPlan>
    {
        public EditTrainingPageViewModel(
            INavigationService navigationService,
            ILogger<EditTrainingPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            ITrainingService trainingService)
            : base(navigationService, logger, dispatcher, sessionService, trainingService)
        {
        }

        protected override string EntityName => Strings.AppStrings.TrainingPlaneEntityName;

        protected override void LoadOnUI(TrainingPlan plan)
        {
            Name = plan.Name;
        }

        protected override TrainingPlan GetUpdateEntity()
        {
            _entity!.Name = this.Name!;
            return _entity;
        }

        protected override bool CanSaveChanges()
        {
            return base.CanSaveChanges() && _entity != null && Name != _entity.Name;
        }
    }
}
