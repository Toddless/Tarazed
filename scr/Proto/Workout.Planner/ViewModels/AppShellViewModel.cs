namespace Workout.Planner.ViewModels
{
    using System.Collections.ObjectModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;

    public class AppShellViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;

        public AppShellViewModel(
            INavigationService navigationService,
            ILogger<AppShellViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService)
            : base(navigationService, logger, dispatcher)
        {
            _sessionService = sessionService;

            // fügt die menuitems hinzu.
            MenuItems.Add(new NavigationEntry() { Title = Strings.AppStrings.SettingsButton, Command = new Command<string>(NavigateTo), CommandParameter = RouteNames.SettingsPage });
            MenuItems.Add(new NavigationEntry() { Title = Strings.AppStrings.LogoutButton, Command = new Command(UserLogoutAsync), CommandParameter = string.Empty });
            MenuItems.Add(new NavigationEntry() { Title = Strings.AppStrings.AboutButton, Command = new Command<string>(NavigateTo), CommandParameter = RouteNames.AboutPage });
        }

        public ObservableCollection<NavigationEntry> MenuItems { get; } = [];

        protected override string? Validate(string collumName)
        {
            return string.Empty;
        }

        private async void UserLogoutAsync()
        {
            try
            {
                await _sessionService.UserLogoutAsync();
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
            }
        }

        private async void NavigateTo(string route)
        {
            await NavigationService.NavigateToOnUIAsync(route);
        }
    }
}
