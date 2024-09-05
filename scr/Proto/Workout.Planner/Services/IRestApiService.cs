namespace Workout.Planner.Services
{
    public interface IRestApiService : IDisposable
    {
        Task<IEnumerable<TV>> GetAsync<TV>(string route, CancellationToken token);

        void Initialize(string baseUrl, Func<Task<bool>>? refreshTokenAsync);

        Task<TV> PostAsync<TU, TV>(string route, TU payload, CancellationToken token);

        Task<TV> PutAsync<TV, TU>(string route, TU payload, CancellationToken token);

        void SetBearerToken(string token);
    }
}
