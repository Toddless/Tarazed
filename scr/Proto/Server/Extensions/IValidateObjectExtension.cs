namespace Server.Extensions
{
    using System.ComponentModel.DataAnnotations;

    public static class IValidateObjectExtension
    {
        public static bool ValidateObject<T>(this T obj, out List<ValidationResult> results)
        {
            ArgumentNullException.ThrowIfNull(obj);

            var validationContext = new ValidationContext(obj, serviceProvider: null, items: null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, validationContext, results, true);
        }
    }
}
