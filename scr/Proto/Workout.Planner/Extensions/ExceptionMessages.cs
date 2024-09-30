namespace Workout.Planner.Extensions
{
    public static class ExceptionMessages
    {
        public const string NotSupportedException = " not supported on uninitialized Proxy.";
        public const string TokenExpired = "Access token is expired during the execution of the code.";
        public const string LoginExpired = "Login is expired.";
        public const string IncorrectEmailOrPassword = "Email or password is incorrect.";
        public const string EmailAlreadyExists = "Email already exists.";
        public const string RefreshTokenNotFound = "Refresh token was not found.";
        public const string RefreshTokenIsExpired = "Refresh token expired.";
        public const string TokenIsExpired = "Token is expired.";
        public const string TokenIsNotFound = "Token is not found.";
        public const string EmailNotExists = "Email not exists.";
        public const string ResetCodeOrEmail = "Reset code or email is incorrect.";
    }
}
