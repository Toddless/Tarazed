namespace Workout.Planner.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INavigationService
    {
        Task NavigateToOnUIAsync(string route, IDictionary<string, object>? parameters = null);

        bool CanNavigate(string route, IDictionary<string, object>? parameters = null);

        Task ShowModalAsync(string route);

        Task CloseModalAsync();

        Task DisplayAlertOnUiAsync(string title, string message, string cancelButton);

        Task<string> DisplayPromtOnUiAsync(string title, string message, string okButton, string cancelButton);
    }
}
