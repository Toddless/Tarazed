namespace Workout.Planner.Models
{
    using DataModel;

    public class MuscleIntensityLevelModel : ObservableObject
    {
        private Muscle _muscle;
        private Intensity _intensity;

        public MuscleIntensityLevelModel()
        {

        }

        public Muscle Muscle
        {
            get => _muscle;
            set => SetProperty(ref _muscle, value);
        }

        public Intensity Intensity
        {
            get => _intensity;
            set => SetProperty(ref _intensity, value);
        }

        public static IEnumerable<MuscleIntensityLevelModel> Import(IEnumerable<MuscleIntensityLevel>? intensities)
        {
            if(intensities == null)
            {
                yield break;
            }

            foreach (var item in intensities)
            {
                yield return new MuscleIntensityLevelModel()
                {
                    Intensity = item.Intensity,
                    Muscle = item.Muscle,
                };
            }
        }
    }
}
