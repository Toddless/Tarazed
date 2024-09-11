namespace Workout.Planner.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services;
    using Workout.Planner.Strings;

    public class EditTrainingPageViewModel : LoadDataBaseViewModel, IQueryAttributable
    {
        private ISessionService _sessionService;
        private ITrainingService _trainingService;
        private TrainingPlan _planModel;
        private string _planName;
        private long? _id;

        public EditTrainingPageViewModel(INavigationService navigationService, ILogger<LoadDataBaseViewModel> logger, IDispatcher dispatcher, ISessionService sessionService, ITrainingService trainingService)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(trainingService);
            ArgumentNullException.ThrowIfNull(sessionService);
            SaveCommand = new Command(SaveChangesAsync, CanSaveChanges);
            _trainingService = trainingService;
            _sessionService = sessionService;
        }

        public string PlanName
        {
            get => _planName;
            set
            {
                if (SetProperty(ref _planName, value))
                {
                    RefreshCommands();
                }
            }
        }

        public Command SaveCommand { get; }

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
            if (query.ContainsKey(nameof(Id)))
            {
                Id = (long?)query[nameof(Id)];
            }
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SaveCommand?.ChangeCanExecute();
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);
                token.ThrowIfCancellationRequested();

                // aber im fall wenn es nichts ausgewählt wurde, wird hier null übergeben, was führt dazu, dass
                // wir alle pläne laden. Diese seite kann sich mit mehrere plane nicht umgehen => wird exception geworfen.
                if (Id.HasValue)
                {
                    var plans = await _trainingService.GetTrainingAsync(false, token, [Id.Value]).ConfigureAwait(false);
                    _planModel = plans.Single();
                    await DispatchToUI(() =>
                    {
                        PlanName = _planModel.Name;
                    }).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingInformation("Loading was canceled", this);
            }
        }

        private async void SaveChangesAsync()
        {
            CancellationToken token = default;
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);
                token = GetCancelationToken();
                if (_planModel != null)
                {
                    if (PlanName == _planModel.Name)
                    {
                        var x = Thread.CurrentThread.CurrentUICulture;

                        await DispatchToUI(() => Shell.Current.CurrentPage.DisplayAlert(AppStrings.AllertInformation, AppStrings.NewNameMatch, AppStrings.OkButton)).ConfigureAwait(false);
                        return;
                    }

                    _planModel.Name = PlanName;

                    await _trainingService.UpdateTrainingAsync(_planModel, token).ConfigureAwait(false);
                }
                else
                {
                    var plan = new TrainingPlan
                    {
                        Name = PlanName,
                    };
                    await _trainingService.CreateTrainingAsync(plan, token).ConfigureAwait(false);
                }

                await DispatchToUI(() => Shell.Current.CurrentPage.DisplayAlert(AppStrings.AllertInformation, AppStrings.ChangesWasSaved, AppStrings.OkButton)).ConfigureAwait(false);

                await DispatchToUI(() => NavigationService.NavigateToAsync(RouteNames.Back)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
            }
            finally
            {
                ReleaseCancelationToken(token);
            }
        }

        private bool CanSaveChanges()
        {
            return !IsBusy && !string.IsNullOrEmpty(PlanName);
        }
    }
}
