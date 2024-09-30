namespace Workout.Planner.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using DataModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class EditTrainingPageViewModel : LoadDataBaseViewModel, IQueryAttributable
    {
        private readonly ISessionService _sessionService;
        private readonly ITrainingService _trainingService;
        private TrainingPlan? _plan;
        private string? _name;
        private long? _id;

        public EditTrainingPageViewModel(
            INavigationService navigationService,
            ILogger<EditTrainingPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            ITrainingService trainingService)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(trainingService);
            ArgumentNullException.ThrowIfNull(sessionService);
            SaveCommand = new Command(SaveChangesAsync, CanSaveChanges);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            _trainingService = trainingService;
            _sessionService = sessionService;
            RegisterProperties();
        }

        public ICommand EntryUnfocusedCommand { get; private set; }

        public Command SaveCommand { get; }

        [PropertyToValidate]
        public string? Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    RefreshCommands();
                }
            }
        }

        public string Titel
        {
            get { return Id.HasValue ? AppStrings.TitelEdit : AppStrings.TitelCreate; }
        }

        private long? Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(nameof(Titel));
                }
            }
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
                token.ThrowIfCancellationRequested();
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);

                // aber im fall wenn es nichts ausgewählt wurde, wird hier null übergeben, was führt dazu, dass
                // wir alle pläne laden. Diese seite kann sich mit mehrere plane nicht umgehen => wird exception geworfen.
                if (Id.HasValue)
                {
                    var plans = await _trainingService.GetDataAsync(false, token, [Id.Value]).ConfigureAwait(false);
                    _plan = plans.Single();
                    await DispatchToUI(() =>
                    {
                        Name = _plan.Name;
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingInformation("Loading was canceled", this);
            }
        }

        protected async void SaveChangesAsync()
        {
            CancellationToken token = default;
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);
                token = GetCancelationToken();
                if (_plan != null)
                {
                    if (Name == _plan.Name)
                    {
                        await NavigationService.DisplayAlertOnUiAsync(AppStrings.Information, AppStrings.NameMatch, AppStrings.OkButton).ConfigureAwait(false);
                        return;
                    }

                    _plan.Name = Name!;

                    await _trainingService.UpdataDataAsync(_plan, token).ConfigureAwait(false);
                }
                else
                {
                    var plan = new TrainingPlan
                    {
                        Name = Name!,
                    };

                    var result = await _trainingService.CreateDataAsync(plan, token).ConfigureAwait(false);
                }

                await NavigationService.DisplayAlertOnUiAsync(AppStrings.Information, AppStrings.Successfull, AppStrings.OkButton).ConfigureAwait(false);

                await NavigationService.NavigateToOnUIAsync(RouteNames.Back).ConfigureAwait(false);
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

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SaveCommand?.ChangeCanExecute();
        }

        protected override string? Validate(string collumName)
        {
            string result = string.Empty;
            switch (collumName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        return AppStrings.IsRequerd;
                    }

                    result = ValidationExtensions.ValidateName(Name);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }

                    break;
                default:
                    return AppStrings.SomethingWrong;
            }

            return null;
        }

        private bool CanSaveChanges()
        {
            return !IsBusy && !HasError;
        }
    }
}
