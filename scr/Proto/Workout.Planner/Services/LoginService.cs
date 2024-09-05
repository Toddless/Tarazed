namespace Workout.Planner.Services
{
    using System.Text;
    using System.Text.Json;
    using DataModel;
    using Workout.Planner.Extensions;

    public class LoginService : ILoginService
    {
        private readonly IRestApiService _restAPIService;
        private readonly ISessionService _sessionService;

        public LoginService(ISessionService sessionService, [FromKeyedServices("UnAuthRestAPI")] IRestApiService restAPIService)
        {
            ArgumentNullException.ThrowIfNull(sessionService);
            ArgumentNullException.ThrowIfNull(restAPIService);
            _restAPIService = restAPIService;
            _sessionService = sessionService;
            _restAPIService.Initialize(RouteNames.BaseURI, null);
            _sessionService.Initialize(this);
        }

        public async Task LoginAsync(UserRequest user)
        {
            var response = await _restAPIService.PostAsync<UserRequest, AccessTokenResponse>(RouteNames.LoginControllerRoute, user, CancellationToken.None).ConfigureAwait(false);
            if (response == null)
            {
                throw new UnauthorizedAccessException(ExceptionMessages.IncorrectEmailOrPassword);
            }

            await _sessionService.SetTokenAsync(response).ConfigureAwait(false);
        }

        public async Task RegisterAsync(UserRequest user)
        {
            using (var client = new HttpClient())
            {
                var json = JsonSerializer.Serialize(user);
                using (var content = new StringContent(json, Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json))
                {
                    using (var response = await client.PostAsync(RouteNames.Register, content).ConfigureAwait(false))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new UnauthorizedAccessException(ExceptionMessages.EmailAlreadyExists);
                        }
                    }
                }
            }
        }
    }
}
