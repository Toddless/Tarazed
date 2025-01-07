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
        private ObservableCollection<MuscleIntensityLevelModel>? _muscleIntensities;
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
            : base(navigationService, logger, dispatcher, sessionService)
        {
            ArgumentNullException.ThrowIfNull(exerciseService);
            ArgumentNullException.ThrowIfNull(unitService);
            SelectedItemCommand = new Command(ExecuteExerciseSelected, CanExecuteExerciseSelected);
            _exerciseService = exerciseService;
            _unitService = unitService;
        }

        public Command SelectedItemCommand { get; }

        /// <summary>
        /// Gets or sets Id des Elterns. Wird beim Aufruf diese Seite gekriegt.
        /// </summary>
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

        /// <summary>
        ///  Gets or sets Kollektion für alle Muskelgrupped mit Belastungsintensitäten einer Übung.
        /// </summary>
        public ObservableCollection<MuscleIntensityLevelModel>? MuscleIntensities
        {
            get => _muscleIntensities;
            set => SetProperty(ref _muscleIntensities, value);
        }

        /// <summary>
        /// Gets or sets Kollektion für alle Übungen des Benutzers.
        /// </summary>
        public ObservableCollection<ExerciseModel>? Exercises
        {
            get => _exercises;
            set => SetProperty(ref _exercises, value);
        }

        /// <summary>
        /// Hier ist Id des Parents gespeichert.
        /// Das Id wird in der Methode LoadDataAsync verwendet,
        /// um die Kinderelementen aus der Datenbank zu laden.
        /// </summary>
        /// <param name="query"> To be added.</param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Id = query.GetValue<long>(NavigationParameterNames.EntityId);
        }

        /// <summary>
        /// Lädt die Daten aus der Datenbank anhang der Übergebene Id.
        /// </summary>
        /// <param name="token">To be added.</param>
        /// <returns>Returns no value.</returns>
        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await EnsureAccesTokenAsync(token).ConfigureAwait(false);

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
                var unit = await _unitService.GetDataAsync(true, token, [Id!.Value]).ConfigureAwait(false);
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

                await DispatchToUI(() =>
                {
                    Exercises = new ObservableCollection<ExerciseModel>(
                    unit.Where(x => x.Id == Id)
                        .SelectMany(x => ExerciseModel.Import(x.Exercises)));
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingException(this, ex);
            }
            finally
            {
                await DispatchToUI(() => ReleaseCancelationToken(token))
                .ConfigureAwait(false);
            }
        }

        protected void ExecuteExerciseSelected()
        {
            MuscleIntensities = new ObservableCollection<MuscleIntensityLevelModel>(
                MuscleIntensityLevelModel.Import(Exercise?.Exercise?.MuscleIntensityLevelId));
        }

        /// <summary>
        /// Stellt sicher, das UI-Thread frei ist.
        /// </summary>
        /// <returns>False, wenn das <seealso cref="CancellationToken"/> nicht freigegeben ist.
        /// Mehr. <seealso cref="BaseViewModel.IsBusy"/></returns>
        protected bool CanExecuteExerciseSelected()
        {
            return !IsBusy;
        }

        protected override string? Validate(string collumName)
        {
            // zur zeit nichts zum validieren
            // entweder in der Zukunft die methode verschieben,
            // oder einfach hier stehen bleiben
            // da es sein kann, dass man die später benötigt
            return string.Empty;
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            foreach (var exercise in Exercises ?? new())
            {
                exercise.RefreshCommands();
            }
        }
    }
}
