namespace Workout.Planner.Services.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Maui.Controls;
    using Workout.Planner.Extensions;

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDispatcher _dispatcher;

        public NavigationService(IServiceProvider serviceProvider, IDispatcher dispatcher)
        {
            _serviceProvider = serviceProvider;
            _dispatcher = dispatcher;
        }

        public async Task NavigateToOnUIAsync(string route, IDictionary<string, object>? parameters = null)
        {
            if (!CanNavigate(route, parameters))
            {
                return;
            }

            if (parameters != null)
            {
                await _dispatcher.DispatchToUIAsync(async () =>
                await Shell.Current.GoToAsync(route, parameters)).ConfigureAwait(false);
            }
            else
            {
                await _dispatcher.DispatchToUIAsync(async () =>
                await Shell.Current.GoToAsync(route)).ConfigureAwait(false);
            }
        }

        public async Task ShowModalAsync(string route)
        {
            var page = Routing.GetOrCreateContent(route, _serviceProvider) as ContentPage;
            if (page == null)
            {
                ArgumentNullException.ThrowIfNull(page);
            }

            await _dispatcher.DispatchToUIAsync(async () =>
            await Shell.Current.Navigation.PushModalAsync(page, false)).ConfigureAwait(false);
        }

        public async Task CloseModalAsync()
        {
            await _dispatcher.DispatchToUIAsync(Shell.Current.Navigation.PopModalAsync).ConfigureAwait(false);
        }

        public bool CanNavigate(string route, IDictionary<string, object>? parameters = null)
        {
            return Shell.Current != null && !string.IsNullOrWhiteSpace(route);
        }

        public async Task DisplayAlertOnUiAsync(string title, string message, string cancelButton)
        {
            await _dispatcher.DispatchToUIAsync(async () =>
            {
                await Shell.Current.DisplayAlert(title, message, cancelButton);
            }).ConfigureAwait(false);
        }

        public async Task<string> DisplayPromtOnUiAsync(string title, string message, string okButton, string cancelButton)
        {
            string userAnswer = string.Empty;
            await _dispatcher.DispatchToUIAsync(async () =>
            {
                userAnswer = await Shell.Current.DisplayPromptAsync(title, message, okButton, cancelButton);
            }).ConfigureAwait(false);
            return userAnswer;
        }
    }
}
