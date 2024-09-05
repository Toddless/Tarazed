namespace Workout.Planner.Extensions
{
    public static class ExceptionMessages
    {
        public const string NotSupportedException = " not supported on uninitialized Proxy.";
        public const string TokenExpired = "Access token ist während der Ausführung des Codes abgelaufen.";
        public const string LoginExpired = "Anmeldung ist abgelaufen.";
        public const string IncorrectEmailOrPassword = "Email or password is incorrect.";
        public const string EmailAlreadyExists = "Email already exists.";
        public const string RefreshTokenNotFound = "Refresh token was not found.";
        public const string RefreshTokenIsExpired = "Refresh token expired.";
        public const string TokenIsExpired = "Token is expired.";
        public const string TokenIsNotFound = "Token is not found.";
    }
}
