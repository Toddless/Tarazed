namespace Workout.Planner.Services
{
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text.Json;
    using Microsoft.Net.Http.Headers;
    using Workout.Planner.Helper;

    public class RestApiService : IRestApiService
    {
        private Func<Task<bool>>? _refreshTokenAsync;
        private HttpClient? _httpClient;

        public RestApiService()
        {
        }

        public async Task<TU> GetUserAsync<TU>(string route, CancellationToken token)
        {
            HttpResponseMessage? request = null;

            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.GetUserAsync) + ExceptionMessages.NotSupportedException);
            }

            request = await _httpClient.GetAsync(route, token).ConfigureAwait(false);
            if (_refreshTokenAsync != null && request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await _refreshTokenAsync.Invoke())
                {
                    request = await _httpClient.GetAsync(route, token).ConfigureAwait(false);
                }
            }

            if (request == null || request.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(ExceptionMessages.LoginExpired + $"StatusCode={new { request?.StatusCode, route }}");
            }

            using (var responce = await request.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
            {
                var result = await JsonSerializer.DeserializeAsync<TU>(responce, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, token).ConfigureAwait(false);

                if (result == null)
                {
                    throw new JsonException(ExceptionMessages.TokenExpired);
                }

                return result;
            }
        }

        public async Task<IEnumerable<TV>> GetAsync<TV>(string route, CancellationToken token)
        {
            HttpResponseMessage? request = null;

            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.GetAsync) + ExceptionMessages.NotSupportedException);
            }

            request = await _httpClient.GetAsync(route, token).ConfigureAwait(false);
            if (_refreshTokenAsync != null && request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await _refreshTokenAsync.Invoke())
                {
                    request = await _httpClient.GetAsync(route, token).ConfigureAwait(false);
                }
            }

            if (request == null || request.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(ExceptionMessages.LoginExpired + $"StatusCode={new { request?.StatusCode, route }}");
            }

            using (var responce = await request.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
            {
                // objecte, die in sich noch ein object enthalten, sind nicht ganz richtig deserialisiert.
                var result = await JsonSerializer.DeserializeAsync<TV[]>(responce, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }, token).ConfigureAwait(false);

                // falls accessToken während der Ausführung des codes abläuft, ist hier result = null und Exceprion wird geworfen
                if (result == null)
                {
                    throw new JsonException(ExceptionMessages.TokenExpired);
                }

                return[.. result];
            }
        }

        public async Task<TV> PostAsync<TU, TV>(string route, TU payload, CancellationToken token)
        {
            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.PostAsync) + ExceptionMessages.NotSupportedException);
            }

            using (var content = JsonContent.Create(payload))
            {
                var request = await _httpClient.PostAsync(route, content, token).ConfigureAwait(false);
                if (_refreshTokenAsync != null && request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (await _refreshTokenAsync.Invoke())
                    {
                        request = await _httpClient!.PostAsync(route, content, token).ConfigureAwait(false);
                    }
                }

                if (request == null || request.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(ExceptionMessages.LoginExpired + $"StatusCode={new { request?.StatusCode, route }}");
                }

                using (var responce = await request.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                {
                    var result = await JsonSerializer.DeserializeAsync<TV>(responce, new JsonSerializerOptions(JsonSerializerDefaults.Web), token).ConfigureAwait(false);

                    // falls accessToken während der Ausführung des codes abläuft, ist hier result = null und Exceprion wird geworfen
                    if (result == null)
                    {
                        throw new JsonException(ExceptionMessages.TokenExpired);
                    }

                    return result;
                }
            }
        }

        public async Task<TV> PutAsync<TV, TU>(string route, TU payload, CancellationToken token)
        {
            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.PutAsync) + ExceptionMessages.NotSupportedException);
            }

            using (var content = JsonContent.Create(payload))
            {
                var request = await _httpClient.PutAsync(route, content, token).ConfigureAwait(false);

                if (_refreshTokenAsync != null && request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (await _refreshTokenAsync.Invoke())
                    {
                        request = await _httpClient!.PutAsync(route, content, token).ConfigureAwait(false);
                    }
                }

                if (request == null || request.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(ExceptionMessages.LoginExpired + $"StatusCode={new { request?.StatusCode, route }}");
                }

                using (var responce = await request.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                {
                    var result = await JsonSerializer.DeserializeAsync<TV>(responce, new JsonSerializerOptions(JsonSerializerDefaults.Web)).ConfigureAwait(false);

                    // falls accessToken während der Ausführung des codes abläuft, ist hier result = null und Exceprion wird geworfen
                    if (result == null)
                    {
                        throw new JsonException(ExceptionMessages.TokenExpired);
                    }

                    return result;
                }
            }
        }

        public async Task<bool> DeleteAsync(string route, CancellationToken token)
        {
            HttpResponseMessage? request = null;

            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.PutAsync) + ExceptionMessages.NotSupportedException);
            }

            request = await _httpClient.DeleteAsync(route, token).ConfigureAwait(false);
            if (_refreshTokenAsync != null && request.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (await _refreshTokenAsync.Invoke())
                {
                    request = await _httpClient.DeleteAsync(route, token).ConfigureAwait(false);
                }
            }

            if (request == null || request.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException(ExceptionMessages.LoginExpired + $"StatusCode={new { request?.StatusCode, route }}");
            }

            return true;
        }

        public void SetBearerToken(string token)
        {
            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.SetBearerToken) + ExceptionMessages.NotSupportedException);
            }

            if (_httpClient.DefaultRequestHeaders.Authorization == null || !Equals($"Bearer {token}"))
            {
                _httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {token}");
            }
        }

        public void RemoveBearerToken(string token)
        {
            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.SetBearerToken) + ExceptionMessages.NotSupportedException);
            }

            if (_httpClient.DefaultRequestHeaders.Authorization == null)
            {
                return;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public void Initialize(string baseUrl, Func<Task<bool>>? refreshTokenAsync)
        {
            _httpClient?.Dispose();
            _refreshTokenAsync = refreshTokenAsync;
            _httpClient = new();
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, System.Net.Mime.MediaTypeNames.Application.Json);
        }

        public void Dispose()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }
    }
}
