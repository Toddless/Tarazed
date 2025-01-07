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

        /// <summary>
        /// Sends a GET request to the specified route.
        /// </summary>
        /// <typeparam name="TV">The type of objects in the returned enumerable.</typeparam>
        /// <param name="route">The endpoint route relative to the base URL of the HTTP client.</param>
        /// <param name="token">A cancellation token to cancel the request if needed.</param>
        /// <returns>The task result contains an enumerable of objects of type <typeparamref name="TV"/>
        /// representing the response data from the API.</returns>
        /// <exception cref="NotSupportedException">Thrown if the HTTP client has not been initialized.</exception>
        /// <exception cref="HttpRequestException">Thrown if the request fails due to an expired login or if the HTTP response status is not OK.</exception>
        /// <exception cref="JsonException">Thrown if the response content could not be deserialized.</exception>
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

                // If the accessToken expires during the execution of the code, result here is null and Exception will be thrown.
                if (result == null)
                {
                    throw new JsonException(ExceptionMessages.TokenExpired);
                }

                return[.. result];
            }
        }

        /// <summary>
        /// Sends a POST request to the specified route.
        /// </summary>
        /// <typeparam name="TU">Request to server. Contains the object to be passed to the server.</typeparam>
        /// <typeparam name="TV">Responce from server. Contains an object received from the server.</typeparam>
        /// <param name="route">The endpoint route relative to the base URL of the HTTP client.</param>
        /// <param name="payload">The data to include in the body of the POST request.</param>
        /// <param name="token">A cancellation token to cancel the request if needed.</param>
        /// <returns>The task result contains an enumerable of objects of type <typeparamref name="TV"/>
        ///  representing the response data from the API.</returns>
        /// <exception cref="NotSupportedException">Thrown if the HTTP client has not been initialized.</exception>
        /// <exception cref="HttpRequestException">Thrown if the request fails due to an expired login or if the HTTP response status is not OK.</exception>
        /// <exception cref="JsonException">Thrown if the response content could not be deserialized.</exception>
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

                    // If the accessToken expires during the execution of the code, result here is null and Exception will be thrown.
                    if (result == null)
                    {
                        throw new JsonException(ExceptionMessages.TokenExpired);
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Sends a PUT request to the specified route.
        /// </summary>
        /// <typeparam name="TV">Responce from server. Contains an object received from the server.</typeparam>
        /// <typeparam name="TU">Request to server. Contains the object to be passed to the server.</typeparam>
        /// <param name="route">The endpoint route relative to the base URL of the HTTP client.</param>
        /// <param name="payload">The data to include in the body of the PUT request.</param>
        /// <param name="token">A cancellation token to cancel the request if needed.</param>
        /// <returns>The task result contains an enumerable of objects of type <typeparamref name="TV"/>
        ///  representing the response data from the API.</returns>
        /// <exception cref="NotSupportedException">Thrown if the HTTP client has not been initialized.</exception>
        /// <exception cref="HttpRequestException">Thrown if the request fails due to an expired login or if the HTTP response status is not OK.</exception>
        /// <exception cref="JsonException">Thrown if the response content could not be deserialized.</exception>
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

                    // If the accessToken expires during the execution of the code, result here is null and Exception will be thrown.
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

        /// <summary>
        /// Set bearer token as default request header for authorisation.
        /// </summary>
        /// <param name="token">Bearer token.</param>
        /// <exception cref="NotSupportedException">Thrown if the HTTP client has not been initialized.</exception>
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

        /// <summary>
        /// Remove the default request headers from the http client.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the HTTP client has not been initialized.</exception>
        public void RemoveBearerToken()
        {
            if (_httpClient == null)
            {
                throw new NotSupportedException(nameof(this.RemoveBearerToken) + ExceptionMessages.NotSupportedException);
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

        /// <summary>
        /// Initializes the HTTP client with the specified base URL and sets a function for token refreshing.
        /// </summary>
        /// <param name="baseUrl">The base URL for the HTTP client, used as the base address for API requests.</param>
        /// <param name="refreshTokenAsync"> An optional asynchronous function that refreshes the access token if it expires.</param>
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
