namespace Workout.Planner.ViewModels
{
    using System.Collections.ObjectModel;
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;

    public class ExercisePageViewModel : LoadDataBaseViewModel, IQueryAttributable
    {
        private readonly IExerciseService _exerciseService;
        private readonly IUnitService _unitService;
        private ObservableCollection<ExerciseModel>? _exercises;
        private ObservableCollection<MuscleIntensityLevelModel>? _muscleIntensity;
        private ExerciseModel? _exercise;
        private long? _id;

        public ExercisePageViewModel(
            INavigationService navigationService,
            ILogger<ExercisePageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IExerciseService exerciseService,
            IUnitService unitService)
            : base(navigationService, logger, dispatcher, sessionService)
        {
            ArgumentNullException.ThrowIfNull(exerciseService);
            ArgumentNullException.ThrowIfNull(unitService);
            SelectedItemCommand = new Command(ExecuteExerciseSelected, CanExecuteExerciseSelected);
            _exerciseService = exerciseService;
            _unitService = unitService;
        }

        public Command SelectedItemCommand { get; }

        public long? Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public ExerciseModel? Exercise
        {
            get => _exercise;
            set
            {
                if (SetProperty(ref _exercise, value))
                {
                    SelectedItemCommand?.Execute(Exercise);
                }
            }
        }

        public ObservableCollection<MuscleIntensityLevelModel>? MuscleIntensity
        {
            get => _muscleIntensity;
            set
            {
                SetProperty(ref _muscleIntensity, value);
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
                await EnsureAccesTokenAsync().ConfigureAwait(false);

                var unit = await _unitService.GetDataAsync(true, token, [Id!.Value]).ConfigureAwait(false);

                await DispatchToUI(() =>
                {
                    Exercises = new ObservableCollection<ExerciseModel>(unit.Where(x => x.Id == Id)
                        .SelectMany(x => ExerciseModel.Import(x.Exercises)));
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

        protected async void ExecuteExerciseSelected()
        {
            CancellationToken token = default;
            try
            {
                token = GetCancelationToken();

                var exercise = await _exerciseService.GetDataAsync(true, token, [Exercise!.Id]).ConfigureAwait(false);

                await DispatchToUI(() =>
                 {
                     MuscleIntensity = new ObservableCollection<MuscleIntensityLevelModel>(exercise.SelectMany(x=> ));
                 }).ConfigureAwait(false);
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
        protected bool CanExecuteExerciseSelected()
        {
            return !IsBusy;
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
            if (Exercises != null)
            {
                foreach (var exercise in Exercises)
                {
                    exercise.RefreshCommands();
                }
            }
        }
    }
}
