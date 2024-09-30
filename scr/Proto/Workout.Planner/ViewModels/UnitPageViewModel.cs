namespace Workout.Planner.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;

    public class UnitPageViewModel : LoadDataBaseViewModel, IQueryAttributable
    {
        private readonly IUnitService _unitService;
        private readonly ITrainingService _trainingService;
        private readonly ISessionService _sessionService;
        private ObservableCollection<UnitModel>? _units;
        private UnitModel _selectedUnit;
        private long? _id;

        public UnitPageViewModel(
            INavigationService navigationService,
            ILogger<LoadDataBaseViewModel> logger,
            IDispatcher dispatcher,
            IUnitService unitService,
            ISessionService sessionService,
            ITrainingService trainingService)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(trainingService);
            ArgumentNullException.ThrowIfNull(sessionService);
            ArgumentNullException.ThrowIfNull(unitService);
            _trainingService = trainingService;
            _sessionService = sessionService;
            _unitService = unitService;
        }

        public long? Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public UnitModel SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                SetProperty(ref _selectedUnit, value);
            }
        }

        public ObservableCollection<UnitModel>? Units
        {
            get => _units;
            set => SetProperty(ref _units, value);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(nameof(Id), out object? value))
            {
                Id = (long?)value;
            }
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                token = GetCancelationToken();

                // laden das plan
                var plan = await _trainingService.GetDataAsync(true, token, [Id!.Value]).ConfigureAwait(false);

                var units = await _unitService.GetDataAsync(false, token).ConfigureAwait(false);

                // mit dem plan werden alle units geladen
                await DispatchToUI(() =>
                {
                    Units = new ObservableCollection<UnitModel>(plan.Where(x => x.Id == Id).SelectMany(x => UnitModel.Import(x.Units)));
                }).ConfigureAwait(false);

                // sammelt man alle ids des units

                // request zum server mit allen ids des units

                // speichern im ObservableCollection
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

        protected override string? Validate(string collumName)
        {
            return string.Empty;
        }
    }
}
