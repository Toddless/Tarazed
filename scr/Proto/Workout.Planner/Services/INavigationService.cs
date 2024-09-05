namespace Workout.Planner.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INavigationService
    {
        Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null);

        bool CanNavigate(string route, IDictionary<string, object>? parameters = null);

        Task PushPopupPageAsync(string route);

        Task PopPopupPageAsync();
    }
}
