namespace Workout.Planner.Converter
{
    using System.Globalization;

    public class StringToBooleanConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"Back Conversion not supported by {GetType()}");
        }
    }
}
