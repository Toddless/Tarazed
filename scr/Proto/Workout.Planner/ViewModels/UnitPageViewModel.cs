namespace Workout.Planner.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;

    public class UnitPageViewModel : LoadDataBaseViewModel, IQueryAttributable
    {
        private readonly ITrainingService _trainingService;
        private readonly IUnitService _unitService;
        private ObservableCollection<UnitModel>? _units;
        private UnitModel? _unit;
        private long? _id;

        public UnitPageViewModel(
            INavigationService navigationService,
            ILogger<UnitPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IUnitService unitService,
            ITrainingService trainingService)
            : base(navigationService, logger, dispatcher, sessionService)
        {
            ArgumentNullException.ThrowIfNull(trainingService);
            ArgumentNullException.ThrowIfNull(unitService);
            SelectUnitCommand = new Command(ExecuteSelectUnit, CanSelectUnit);
            AddUnitCommand = new Command(ExecuteAddUnit, CanAddUnit);
            _trainingService = trainingService;
            _unitService = unitService;
        }

        public Command AddUnitCommand { get; }

        public Command SelectUnitCommand { get; }

        public long? Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public UnitModel? Unit
        {
            get => _unit;
            set
            {
                if (SetProperty(ref _unit, value))
                {
                    SelectUnitCommand.Execute(Unit);
                }
            }
        }

        public ObservableCollection<UnitModel>? Units
        {
            get => _units;
            set => SetProperty(ref _units, value);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Id = query.GetValue<long>(NavigationParameterNames.EntityId);
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                token = GetCancelationToken();

                // wie kommen hier von der seite "Home" mit dem id des trainingsplans und holen uns das plan mit allen units
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
                var trainingPlan = await _trainingService.GetDataAsync(true, token, [Id!.Value]).ConfigureAwait(false);
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

                // und hier werden die im UnitModel umwandeln
                await DispatchToUI(() =>
                {
                    Units = new ObservableCollection<UnitModel>(trainingPlan.Where(x => x.Id == Id)
                        .SelectMany(x => UnitModel.Import(x.Units, EditUnitAsync, CanEditUnit, DeleteUnitAsync, CanDeleteUnit)));
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingException(this, ex);
            }
            finally
            {
                await DispatchToUI(() => ReleaseCancelationToken(token)).ConfigureAwait(false);
            }
        }

        protected async Task DeleteUnitAsync(UnitModel model)
        {
            CancellationToken token = default;
            try
            {
                // noch bearbeiten
                if(model.Unit == null)
                {
                    return;
                }

                token = GetCancelationToken();

                await EnsureAccesTokenAsync().ConfigureAwait(false);

                var result = _unitService.DeleteDataAsync([model.Unit.Id], token);
                if (!result)
                {
                    return;
                }

                await LoadDataAsync(token);
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
            }
            finally
            {
                await DispatchToUI(() => ReleaseCancelationToken(token)).ConfigureAwait(false);
            }
        }

        protected async void ExecuteSelectUnit()
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.ExercisePage,
                new Dictionary<string, object> { { NavigationParameterNames.EntityId, _unit!.Id } }).ConfigureAwait(false);
        }

        protected async Task EditUnitAsync(UnitModel model)
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.EditUnitPage,
                new Dictionary<string, object>
                {
                    { NavigationParameterNames.EntityId, model.Unit!.Id },
                    { NavigationParameterNames.RelatedId, Id! },
                }).ConfigureAwait(false);
        }

        protected async void ExecuteAddUnit()
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.EditUnitPage,
                new Dictionary<string, object> { { NavigationParameterNames.RelatedId, Id! } }).ConfigureAwait(false);
        }

        protected override string? Validate(string collumName)
        {
            // zur zeit nichts zum validieren
            // entweder in der Zukunft die methode verschieben, oder einfach hier stehen bleiben
            // da es sein kann, dass man die später benötigt
            return string.Empty;
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SelectUnitCommand?.ChangeCanExecute();
            AddUnitCommand?.ChangeCanExecute();

            if (Units != null)
            {
                foreach (var unit in Units)
                {
                    unit.RefreshCommands();
                }
            }
        }

        private bool CanSelectUnit()
        {
            return !IsBusy;
        }

        private bool CanAddUnit()
        {
            return !IsBusy;
        }

        private bool CanEditUnit(UnitModel model)
        {
            return !IsBusy && model != null;
        }

        private bool CanDeleteUnit(UnitModel model)
        {
            return !IsBusy && model != null;
        }
    }
}
