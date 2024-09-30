namespace Workout.Planner.Services.Contracts
{
    using DataModel;
    using Workout.Planner.Extensions;

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

        public async Task SetTokenAsync(AccessTokenResponse responseToken)
        {
            SecureStorage.Default.RemoveAll();
            var accesTokenExpireTime = DateTime.UtcNow.Ticks + TimeSpan.FromSeconds(responseToken.ExpiresIn).Ticks;
            var refreshTokenExpireTime = DateTime.UtcNow.Ticks + TimeSpan.FromMinutes(30).Ticks;
            await SecureStorage.Default.SetAsync("accessToken", responseToken.AccessToken).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("tokenType", responseToken.TokenType).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("refreshToken", responseToken.RefreshToken).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("AccesTokenExpireIn", accesTokenExpireTime.ToString()).ConfigureAwait(false);
            await SecureStorage.Default.SetAsync("RefreshTokenExpireIn", refreshTokenExpireTime.ToString()).ConfigureAwait(false);

            _restAPIService.SetBearerToken(responseToken.AccessToken);
        }

        public async Task UserLogoutAsync()
        {
            var token = await SecureStorage.Default.GetAsync("accessToken").ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.TokenIsNotFound);
            }

            _restAPIService.RemoveBearerToken(token);
            SecureStorage.Default.RemoveAll();
            await _navigationService.NavigateToOnUIAsync(RouteNames.LoginPage).ConfigureAwait(false);
        }

        public async Task EnsureAccessTokenNotExpiredAsync()
        {
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

            var refresh = new AccessTokenResponse { RefreshToken = refreshToken };

            var request = await _restAPIService.PostAsync<AccessTokenResponse, AccessTokenResponse>(RouteNames.Refresh, refresh, CancellationToken.None).ConfigureAwait(false);
            await SetTokenAsync(request).ConfigureAwait(false);
            return true;
        }

        private static void ParseCurrentTimeAndTokenExpirationInTicks(string? tokenExpiration, out long expires, out long currentTime)
        {
            long.TryParse(tokenExpiration, out expires);
            currentTime = DateTime.UtcNow.Ticks;
        }
    }
}
