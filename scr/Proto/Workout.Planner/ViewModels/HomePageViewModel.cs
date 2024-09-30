namespace Workout.Planner.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class HomePageViewModel : LoadDataBaseViewModel
    {
        private readonly ITrainingService _trainingService;
        private readonly ISessionService _sessionService;

        private ObservableCollection<TrainingPlanModel>? _plans;
        private TrainingPlanModel? _selectedPlan;

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

            SelectItemCommand = new Command(async () => await SelectPlanAsync(), CanSelectPlan);
            AddCommand = new Command(AddPlan, CanAddPlan);
            _trainingService = trainingService;
            _sessionService = sessionService;
        }

        public Command AddCommand { get; }

        public Command SelectItemCommand { get; }

        public TrainingPlanModel? SelectedPlan
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

        public ObservableCollection<TrainingPlanModel>? Plans
        {
            get => _plans;
            set => SetProperty(ref _plans, value);
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

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

        private async Task EditPlanAsync(TrainingPlanModel model)
        {
            await DispatchToUI(() =>
            NavigationService.NavigateToOnUIAsync(
                RouteNames.EditTrainingPage,
                new Dictionary<string, object> { { nameof(TrainingPlan.Id), model!.Id } }))
                .ConfigureAwait(false);
        }

        private async Task DeletePlanAsync(TrainingPlanModel model)
        {
            CancellationToken token = default;
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);
                token = GetCancelationToken();
                var result = await _trainingService.DeleteDataAsync([model.Plan.Id], token).ConfigureAwait(false);

                if (result == true)
                {
                    await DispatchToUI(() => Shell.Current.CurrentPage.DisplayAlert(AppStrings.Information, AppStrings.Deleted, AppStrings.OkButton)).ConfigureAwait(false);
                }
                else
                {
                    await DispatchToUI(() => Shell.Current.CurrentPage.DisplayAlert(AppStrings.Information, AppStrings.SomethingWrong, AppStrings.OkButton)).ConfigureAwait(false);
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

        private async Task SelectPlanAsync()
        {
            await DispatchToUI(() =>
            NavigationService.NavigateToOnUIAsync(
                RouteNames.UnitPage,
                new Dictionary<string, object> { { nameof(TrainingPlan.Id), _selectedPlan!.Id } }))
                .ConfigureAwait(false);
        }

        private async void AddPlan()
        {
            await DispatchToUI(() =>
           NavigationService.NavigateToOnUIAsync(RouteNames.EditTrainingPage)).ConfigureAwait(false);
        }

        private bool CanAddPlan() => !IsBusy;

        private bool CanSelectPlan()
        {
            return !IsBusy;
        }

        private bool CanEditPlan(TrainingPlanModel model)
        {
            return model != null && !IsBusy;
        }

        private bool CanDeletePlan(TrainingPlanModel model)
        {
            return model != null && !IsBusy;
        }

        protected override string? Validate(string collumName) => throw new NotImplementedException();
    }
}
