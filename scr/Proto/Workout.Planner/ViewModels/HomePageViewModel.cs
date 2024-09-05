namespace Workout.Planner.ViewModels
{
    using System.Threading;
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services;

    public class HomePageViewModel : LoadDataBaseViewModel
    {
        private readonly ITrainingService _trainingService;
        private readonly ISessionService _sessionService;
        private IEnumerable<TrainingPlan>? _plans;
        private TrainingPlan? _selectedPlan;

        public HomePageViewModel(
            ITrainingService trainingService,
            ILogger<HomePageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            INavigationService navigationService)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(sessionService);
            ArgumentNullException.ThrowIfNull(trainingService);

            SelectItemCommand = new Command<TrainingPlan>(async (obj) => await PlanSelectedAsync(obj));
            AddCommand = new Command(AddPlan, CanAddPlan);
            _trainingService = trainingService;
            _sessionService = sessionService;
        }

        public Command AddCommand { get; }

        public TrainingPlan? SelectedPlan
        {
            get => _selectedPlan;
            set
            {
                if (SetProperty(ref _selectedPlan, value))
                {
                    SelectItemCommand.Execute(SelectedPlan);
                }
            }
        }

        public Command SelectItemCommand { get; }

        public IEnumerable<TrainingPlan>? Plans
        {
            get => _plans;
            set => SetProperty(ref _plans, value);
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                await _sessionService.EnsureTokenAsync().ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException)
            {
                Logger.LoggingInformation("Acces token and refresh token are expired", this);
                await DispatchToUI(() => NavigationService.PushPopupPageAsync(RouteNames.LoginPage)).ConfigureAwait(false);
            }

            try
            {
                token.ThrowIfCancellationRequested();

                var plans = await _trainingService.GetTrainingAsync(true, token).ConfigureAwait(false);
                await DispatchToUI(() =>
                {
                    Plans = plans;
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingInformation("Loading was canceled", this);
            }
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SelectItemCommand?.ChangeCanExecute();
            AddCommand?.ChangeCanExecute();
        }

        private async Task PlanSelectedAsync(TrainingPlan plan)
        {
            await DispatchToUI(() => NavigationService.NavigateToAsync(RouteNames.UnitPage)).ConfigureAwait(false);
        }

        private void AddPlan()
        {
        }

        private bool CanAddPlan() => !IsBusy;
    }
}
