namespace Workout.Planner.Services.Contracts
{
    using DataModel;
    using Workout.Planner.Helper;

    public class SessionService : ISessionService
    {
        private readonly IRestApiService _restAPIService;
        private readonly INavigationService _navigationService;
        private LoginService? _loginService;

        public SessionService([FromKeyedServices("AuthRestAPI")] IRestApiService restAPIService, INavigationService navigationService)
        {
            ArgumentNullException.ThrowIfNull(restAPIService);
            ArgumentNullException.ThrowIfNull(navigationService);
            _restAPIService = restAPIService;
            _navigationService = navigationService;
            _restAPIService.Initialize(RouteNames.BaseURI, RefreshTokenAsync);
        }

        public void Initialize(LoginService loginService)
        {
            ArgumentNullException.ThrowIfNull(loginService);
            _loginService = loginService;
        }

        /// <summary>
        /// Store user Access token, Refresh token and expiration time in SecureStorage.
        /// The expiration of the access token is always defined in seconds.
        /// The refresh token expiration time can be anything from a few milliseconds to days.
        /// </summary>
        /// <param name="responseToken">Responce from server. Includes user access, refresh token and expiry time.</param>
        /// <returns>The task has no return value unless it fails.</returns>
        public async Task SetTokenAsync(TokenHandlingModel responseToken)
        {
            SecureStorage.Default.RemoveAll();
            var accesTokenExpireTime = DateTime.UtcNow.Ticks + TimeSpan.FromSeconds(responseToken.ExpiresIn).Ticks;
            var refreshTokenExpireTime = DateTime.UtcNow.Ticks + TimeSpan.FromDays(30).Ticks;
            await SecureStorage.Default.SetAsync("accessToken", responseToken.AccessToken).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("tokenType", responseToken.TokenType).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("refreshToken", responseToken.RefreshToken).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("AccesTokenExpireIn", accesTokenExpireTime.ToString()).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("RefreshTokenExpireIn", refreshTokenExpireTime.ToString()).ConfigureAwait(false);

            _restAPIService.SetBearerToken(responseToken.AccessToken);
        }

        /// <summary>
        ///  Clears the SecureStorage and navigates to the login page after the user logs out.
        /// </summary>
        /// <returns>A task representing the asynchronous logout operation.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when no access token is found in SecureStorage, indicating the user is not logged in.</exception>
        public async Task UserLogoutAsync()
        {
            var token = await SecureStorage.Default.GetAsync("accessToken").ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.TokenIsNotFound);
            }

            _restAPIService.RemoveBearerToken();
            SecureStorage.Default.RemoveAll();
            await _navigationService.NavigateToOnUIAsync(RouteNames.LoginPage).ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures that the access token stored in SecureStorage is valid and not expired.
        /// If the token is expired, a refresh operation is triggered.
        /// </summary>
        /// <param name="token">A cancellation token to cancel the operation if needed.</param>
        /// <returns>A task representing the asynchronous validation and refresh operation.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown if no access token or expiration information is found in SecureStorage,
        /// indicating that the user is not logged in or the token data is missing.</exception>
        /// <exception cref="OperationCanceledException">Thrown if the operation is canceled through the provided cancellation token.</exception>
        public async Task EnsureAccessTokenNotExpiredAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var accessToken = await SecureStorage.Default.GetAsync("accessToken").ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.TokenIsNotFound);
            }

            var accessTokenExpire = await SecureStorage.Default.GetAsync("AccesTokenExpireIn").ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(accessTokenExpire))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.TokenIsNotFound);
            }

            ParseCurrentTimeAndTokenExpirationInTicks(accessTokenExpire, out long expires, out long currentTime);
            if (currentTime > expires)
            {
                await RefreshTokenAsync().ConfigureAwait(false);
                return;
            }

            _restAPIService.SetBearerToken(accessToken);
        }

        /// <summary>
        /// Refreshes the access token using the refresh token stored in SecureStorage.
        /// Checks if the refresh token is still valid before initiating the refresh process.
        /// </summary>
        /// <returns>The task result is a boolean indicating,
        /// whether the token was successfully refreshed (<see langword="true"/> if refreshed successfully).</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown if the refresh token is expired or missing.</exception>
        public async Task<bool> RefreshTokenAsync()
        {
            var refreshTokenExpired = await SecureStorage.Default.GetAsync("RefreshTokenExpireIn").ConfigureAwait(false);
            ParseCurrentTimeAndTokenExpirationInTicks(refreshTokenExpired, out long expires, out long currentTime);
            if (currentTime > expires)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.RefreshTokenIsExpired);
            }

            var refreshToken = await SecureStorage.Default.GetAsync("refreshToken").ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.RefreshTokenNotFound);
            }

            var refresh = new TokenHandlingModel { RefreshToken = refreshToken };

            var request = await _restAPIService.PostAsync<TokenHandlingModel, TokenHandlingModel>(RouteNames.Refresh, refresh, CancellationToken.None).ConfigureAwait(false);
            await SetTokenAsync(request).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// Convert expiration time stored as string to long and calculate current time in ticks.
        /// </summary>
        /// <param name="tokenExpiration"> Expiration time as string.</param>
        /// <param name="expires"> Converted to long expiration time.</param>
        /// <param name="currentTime">Converted to ticks current time.</param>
        private static void ParseCurrentTimeAndTokenExpirationInTicks(string? tokenExpiration, out long expires, out long currentTime)
        {
            long.TryParse(tokenExpiration, out expires);
            currentTime = DateTime.UtcNow.Ticks;
        }
    }
}
