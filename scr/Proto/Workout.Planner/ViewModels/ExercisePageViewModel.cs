namespace Workout.Planner.ViewModels
{
    using System.Collections.ObjectModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;

    public class ExercisePageViewModel : LoadDataBaseViewModel, IQueryAttributable
    {
        private readonly IExerciseService _exerciseService;
        private readonly IUnitService _unitService;
        private readonly ISessionService _sessionService;
        private ObservableCollection<ExerciseModel>? _exercises;
        private ExerciseModel? _exercise;
        private long? _id;

        public ExercisePageViewModel(
            INavigationService navigationService,
            ILogger<ExercisePageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IExerciseService exerciseService,
            IUnitService unitService)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(exerciseService);
            ArgumentNullException.ThrowIfNull(sessionService);
            ArgumentNullException.ThrowIfNull(unitService);
            SelectExerciseCommand = new Command(SelectExercise, CanSelect);
            AddExerciseCommand = new Command(AddExercise, CanAdd);
            _unitService = unitService;
            _exerciseService = exerciseService;
            _sessionService = sessionService;
        }

        public Command SelectExerciseCommand { get; }

        public Command AddExerciseCommand { get; }

        public long? Id
        {
            get => _id;
            set
            {
                SetProperty(ref _id, value);
            }
        }

        public ExerciseModel? Exercise
        {
            get => _exercise;
            set
            {
                if (SetProperty(ref _exercise, value))
                {
                    SelectExerciseCommand.Execute(Exercise);
                }
            }
        }

        public ObservableCollection<ExerciseModel>? Exercises
        {
            get => _exercises;
            set => SetProperty(ref _exercises, value);
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
                var exercise = await _unitService.GetDataAsync(true, token, [Id!.Value]).ConfigureAwait(false);

                await DispatchToUI(() =>
                {
                    Exercises = new ObservableCollection<ExerciseModel>(exercise.Where(x => x.Id == Id)
                        .SelectMany(x => ExerciseModel.Import(x.Exercises, EditExerciseAsync, CanEditUnit, DeleteExerciseAsync, CanDeleteUnit)));
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

        protected async Task DeleteExerciseAsync(ExerciseModel model)
        {
            CancellationToken token = default;
            try
            {
                token = GetCancelationToken();
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);

                var result = await _exerciseService.DeleteDataAsync([model.Exercise.Id], token).ConfigureAwait(false);
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

        protected async void SelectExercise()
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.ExerciseDetailPage,
                new Dictionary<string, object> { { nameof(Exercise.Id), _exercise!.Id } }).ConfigureAwait(false);
        }

        protected async Task EditExerciseAsync(ExerciseModel model)
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.EditExercisePage,
                new Dictionary<string, object>
                {
                    { NavigationParameterNames.EntityId, model.Exercise!.Id },
                    { NavigationParameterNames.RelatedId, Id! },
                }).ConfigureAwait(false);
        }

        protected async void AddExercise()
        {
            await NavigationService.NavigateToOnUIAsync(
                RouteNames.EditExercisePage,
                new Dictionary<string, object> { { NavigationParameterNames.RelatedId, Id! } }).ConfigureAwait(false);
        }

        protected override string? Validate(string collumName)
        {
            return string.Empty;
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SelectExerciseCommand?.ChangeCanExecute();
            AddExerciseCommand?.ChangeCanExecute();

            if (Exercises != null)
            {
                foreach (var exercise in Exercises)
                {
                    exercise.RefreshCommands();
                }
            }
        }

        private bool CanSelect()
        {
            return !IsBusy;
        }

        private bool CanAdd()
        {
            return !IsBusy;
        }

        private bool CanEditUnit(ExerciseModel model)
        {
            return !IsBusy && model != null;
        }

        private bool CanDeleteUnit(ExerciseModel model)
        {
            return !IsBusy && model != null;
        }
    }
}
