namespace Workout.Planner.Helper
{
    using DataModel;

    public static class IntensityHelper
    {
        public static string GetIntensity(Intensity intensity)
        {
            return intensity switch
            {
                Intensity.None => "st1",
                Intensity.Low => "st2",
                Intensity.Moderate => "st3",
                Intensity.High => "st4",
                Intensity.VeryHigh => "st5",
                Intensity.Extremly => "st6",
                _ => string.Empty
            };
        }
    }
}
