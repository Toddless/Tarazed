namespace Workout.Planner.Models
{
    using DataModel;

    public class MuscleIntensityLevelModel : ObservableObject
    {
        private string? _muscle;
        private Intensity _intensity;

        public MuscleIntensityLevelModel()
        {
        }

        public string? Muscle
        {
            get => _muscle;
            set => SetProperty(ref _muscle, value);
        }

        public Intensity Intensity
        {
            get => _intensity;
            set => SetProperty(ref _intensity, value);
        }

        /// <summary>
        /// Wandelt das MuscleIntensityLevel ins MuscleIntensityLevelModel um.
        /// </summary>
        /// <param name="intensities">Kollektion mit Muskelgruppen und Intensitäten.</param>
        /// <returns>Gibt das neue MuscleIntensityLevelModel zurück.</returns>
        public static IEnumerable<MuscleIntensityLevelModel> Import(IEnumerable<MuscleIntensityLevel>? intensities)
        {
            if (intensities == null)
            {
                yield break;
            }

            foreach (var item in intensities)
            {
                yield return new MuscleIntensityLevelModel()
                {
                    Intensity = item.Intensity,
                    Muscle = item.Muscle.ToString(),
                };
            }
        }
    }
}
