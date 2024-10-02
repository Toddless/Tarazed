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
                _entity!.TrainingPlanId = (long)RelatedId!;
            }

            _entity!.Name = this.Name!;
            return _entity;
        }

        protected override bool CanSaveChanges()
        {
            return base.CanSaveChanges() && _entity != null && Name != _entity.Name;
        }
    }
}
