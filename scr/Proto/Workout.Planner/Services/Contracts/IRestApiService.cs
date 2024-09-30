namespace Workout.Planner.Services
{
    public interface IRestApiService : IDisposable
    {
        void Initialize(string baseUrl, Func<Task<bool>>? refreshTokenAsync);

        Task<IEnumerable<TV>> GetAsync<TV>(string route, CancellationToken token);

        Task<TU> GetUserAsync<TU>(string route, CancellationToken token);

        Task<TV> PostAsync<TU, TV>(string route, TU payload, CancellationToken token);

        Task<TV> PutAsync<TV, TU>(string route, TU payload, CancellationToken token);

        Task<bool> DeleteAsync(string route, CancellationToken token);

        void RemoveBearerToken(string token);

        void SetBearerToken(string token);
    }
}
