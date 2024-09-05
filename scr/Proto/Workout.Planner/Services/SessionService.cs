namespace Workout.Planner.Services
{
    using DataModel;
    using Workout.Planner.Extensions;

    public class SessionService : ISessionService
    {
        private IRestApiService _restAPIService;
        private LoginService? _loginService;

        public SessionService([FromKeyedServices("AuthRestAPI")] IRestApiService restAPIService)
        {
            ArgumentNullException.ThrowIfNull(restAPIService);
            _restAPIService = restAPIService;
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
            await SecureStorage.Default.SetAsync("accessToken", responseToken.AccessToken);
            await SecureStorage.Default.SetAsync("tokenType", responseToken.TokenType);
            await SecureStorage.Default.SetAsync("refreshToken", responseToken.RefreshToken);
            await SecureStorage.Default.SetAsync("AccesTokenExpireIn", accesTokenExpireTime.ToString());
            await SecureStorage.Default.SetAsync("RefreshTokenExpireIn", refreshTokenExpireTime.ToString());

            _restAPIService.SetBearerToken(responseToken.AccessToken);
        }

        public async Task EnsureTokenAsync()
        {
            var accessToken = await SecureStorage.Default.GetAsync("accessToken");
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.TokenIsNotFound);
            }

            var accessTokenExpire = await SecureStorage.Default.GetAsync("AccesTokenExpireIn");
            if (string.IsNullOrWhiteSpace(accessTokenExpire))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.TokenIsNotFound);
            }

            long expires, currentTime;
            ParseCurrentTimeAndTokenExpirationInTicks(accessTokenExpire, out expires, out currentTime);
            if (currentTime > expires)
            {
                await RefreshTokenAsync();
                return;
            }

            _restAPIService.SetBearerToken(accessToken);
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshTokenExpired = await SecureStorage.Default.GetAsync("RefreshTokenExpireIn");
            long expires, currentTime;
            ParseCurrentTimeAndTokenExpirationInTicks(refreshTokenExpired, out expires, out currentTime);
            if (currentTime > expires)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.RefreshTokenIsExpired);
            }

            var refreshToken = await SecureStorage.Default.GetAsync("refreshToken");
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new UnauthorizedAccessException(ExceptionMessages.RefreshTokenNotFound);
            }

            var refresh = new AccessTokenResponse { RefreshToken = refreshToken };

            var request = await _restAPIService.PostAsync<AccessTokenResponse, AccessTokenResponse>(RouteNames.Refresh, refresh, CancellationToken.None);
            await SetTokenAsync(request);
            return true;
        }

        private static void ParseCurrentTimeAndTokenExpirationInTicks(string? tokenExpiration, out long expires, out long currentTime)
        {
            long.TryParse(tokenExpiration, out expires);
            currentTime = DateTime.UtcNow.Ticks;
        }
    }
}
