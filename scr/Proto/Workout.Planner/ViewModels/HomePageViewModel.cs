namespace Workout.Planner.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class HomePageViewModel : LoadDataBaseViewModel
    {
        private readonly ITrainingService _trainingService;
        private ObservableCollection<TrainingPlanModel>? _plans;
        private TrainingPlanModel? _plan;

        public HomePageViewModel(
            ITrainingService trainingService,
            ILogger<HomePageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            INavigationService navigationService)
            : base(navigationService, logger, dispatcher, sessionService)
        {
            ArgumentNullException.ThrowIfNull(trainingService);

            SelectItemCommand = new Command(ExecuteSelectPlan, CanSelectPlan);
            AddCommand = new Command(ExecuteAddPlan, CanAddPlan);
            _trainingService = trainingService;
        }

        public Command SelectItemCommand { get; }

        public Command AddCommand { get; }

        public ObservableCollection<TrainingPlanModel>? Plans
        {
            get => _plans;
            set { SetProperty(ref _plans, value); }
        }

        public TrainingPlanModel? Plan
        {
            get => _plan;
            set
            {
                if (SetProperty(ref _plan, value))
                {
                    SelectItemCommand.Execute(Plan);
                }
            }
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await EnsureAccesTokenAsync().ConfigureAwait(false);
                var plans = await _trainingService.GetDataAsync(false, token).ConfigureAwait(false);
                await DispatchToUI(() =>
                {
                    Plans = new ObservableCollection<TrainingPlanModel>(plans.Select(x => TrainingPlanModel.Import(x, EditPlanAsync, CanEditPlan, DeletePlanAsync, CanDeletePlan)));
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingInformation("Loading was canceled", this);
            }
            finally
            {
                await DispatchToUI(() => ReleaseCancelationToken(token)).ConfigureAwait(false);
            }
        }

        protected async Task DeletePlanAsync(TrainingPlanModel model)
        {
            CancellationToken token = default;
            try
            {
                await EnsureAccesTokenAsync().ConfigureAwait(false);
                token = GetCancelationToken();
                var result = await _trainingService.DeleteDataAsync([model.Plan.Id], token).ConfigureAwait(false);
                if (result == true)
                {
                    // todo: popups loswerden
                    await NavigationService.DisplayAlertOnUiAsync(
                        AppStrings.Information,
                        AppStrings.Deleted,
                        AppStrings.OkButton).ConfigureAwait(false);
                }
                else
                {
                    // todo: popups loswerden
                    await NavigationService.DisplayAlertOnUiAsync(
                        AppStrings.Information,
                        AppStrings.SomethingWrong,
                        AppStrings.OkButton).ConfigureAwait(false);
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

        protected async void ExecuteSelectPlan()
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.UnitPage,
                new Dictionary<string, object> { { NavigationParameterNames.EntityId, _plan!.Id } })
                .ConfigureAwait(false);
        }

        protected async Task EditPlanAsync(TrainingPlanModel model)
        {
           await NavigationService.NavigateToOnUIAsync(
               RouteNames.EditTrainingPage,
               new Dictionary<string, object> { { NavigationParameterNames.EntityId, model.Plan!.Id } }).ConfigureAwait(false);
        }

        protected async void ExecuteAddPlan()
        {
           await NavigationService.NavigateToOnUIAsync(RouteNames.EditTrainingPage).ConfigureAwait(false);
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SelectItemCommand?.ChangeCanExecute();
            AddCommand?.ChangeCanExecute();

            if (Plans != null)
            {
                foreach (var plan in Plans)
                {
                    plan.RefreshCommands();
                }
            }
        }

        protected override string? Validate(string collumName)
        {
            return string.Empty;
        }

        private bool CanEditPlan(TrainingPlanModel model)
        {
            return !IsBusy && model != null;
        }

        private bool CanDeletePlan(TrainingPlanModel model)
        {
            return !IsBusy && model != null;
        }

        private bool CanAddPlan() => !IsBusy;

        private bool CanSelectPlan()
        {
            return !IsBusy;
        }
    }
}
