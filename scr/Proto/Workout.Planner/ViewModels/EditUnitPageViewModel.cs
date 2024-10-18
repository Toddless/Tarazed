namespace Workout.Planner.ViewModels
{
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Microsoft.Maui.Dispatching;
    using Workout.Planner.Services.Contracts;

    public class EditUnitPageViewModel : EditViewModelBase<Unit>
    {
        public EditUnitPageViewModel(
            INavigationService navigationService,
            ILogger<EditUnitPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IUnitService service)
            : base(navigationService, logger, dispatcher, sessionService, service)
        {
        }

        protected override string EntityName => Strings.AppStrings.UnitEntityName;

        protected override void LoadOnUI(Unit unit)
        {
            Name = unit.Name;
        }

        protected override Unit GetUpdateEntity()
        {
            if (RelatedId != 0)
            {
                Entity!.TrainingPlanId = (long)RelatedId!;
            }

            Entity!.Name = this.Name!;
            return Entity;
        }

        protected override bool CanSaveChanges()
        {
            return base.CanSaveChanges() && Entity != null && Name != Entity.Name;
        }
    }
}
