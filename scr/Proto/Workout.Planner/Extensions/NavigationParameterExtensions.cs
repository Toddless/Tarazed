namespace Workout.Planner.Extensions
{
    public static class NavigationParameterExtensions
    {
        public static TU? GetValue<TU>(this IDictionary<string, object> query, string parameterName, TU? defaultValue = default)
        {
            try
            {
                TU? result = defaultValue;
                if (query.TryGetValue(parameterName, out object? value))
                {
                    result = (TU)value;
                }

                return result;
            }
            catch (Exception)
            {
                // todo: logger
                return defaultValue;
            }
        }
    }
}
