namespace Workout.Planner.Extensions
{
    using System.Text.RegularExpressions;
    using Workout.Planner.Strings;

    /// <summary>
    /// Hauptstelle wo man alle benötigte properties validiert.
    /// </summary>
    public static partial class ValidationExtensions
    {
        public static string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return AppStrings.IsRequerd;
            }

            Regex emailRegex = EmailRegex();
            var valid = emailRegex.IsMatch(email.Trim());
            if (!valid)
            {
                return AppStrings.EmailFormatWrong;
            }

            return string.Empty;
        }

        public static string ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return AppStrings.IsRequerd;
            }

            bool isWhiteSpace = password.Any(char.IsWhiteSpace);
            bool isDigit = password.Any(char.IsDigit);
            bool isUpper = password.Any(char.IsUpper);
#if DEBUG
            bool isLengthFullfilled = password.Length > 3;
#else
            bool isLengthFullfilled = password.Length > 12;
#endif
            bool isValid = isDigit && isUpper && isLengthFullfilled && !isWhiteSpace;

            if (!isValid)
            {
                return AppStrings.PasswordFormatWrong;
            }

            return string.Empty;
        }

        public static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return AppStrings.IsRequerd;
            }

            if (name.Trim().Length > 50 || name.Trim().Length < 4)
            {
                return AppStrings.NameFormatWrong;
            }

            return string.Empty;
        }

        [GeneratedRegex(@"^[\w\.\-!#$%&'*+/=?^_`{|}~\u00C0-\u024F]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        private static partial Regex EmailRegex();
    }
}
