namespace Workout.Planner.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Maui.Controls;

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task NavigateToAsync(string route, IDictionary<string, object>? parameters = null)
        {
            if (!CanNavigate(route, parameters))
            {
                return;
            }

            if (parameters != null)
            {
                await Shell.Current.GoToAsync(route, parameters).ConfigureAwait(false);
            }
            else
            {
                await Shell.Current.GoToAsync(route).ConfigureAwait(false);
            }
        }

        public async Task PushPopupPageAsync(string route)
        {
            var page = Routing.GetOrCreateContent(route, _serviceProvider) as ContentPage;
            if (page == null)
            {
                ArgumentNullException.ThrowIfNull(page);
            }

            await Shell.Current.Navigation.PushModalAsync(page, false).ConfigureAwait(false);
        }

        public async Task PopPopupPageAsync()
        {
            await Shell.Current.Navigation.PopModalAsync().ConfigureAwait(false);
        }

        public bool CanNavigate(string route, IDictionary<string, object>? parameters = null)
        {
            return Shell.Current != null && !string.IsNullOrWhiteSpace(route);
        }
    }
}
