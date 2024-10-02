namespace Workout.Planner.ViewModels
{
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Services.Contracts;

    public abstract class LoadDataBaseViewModel : BaseViewModel
    {
        protected LoadDataBaseViewModel(
            INavigationService navigationService,
            ILogger<LoadDataBaseViewModel> logger,
            IDispatcher dispatcher)
            : base(navigationService, logger, dispatcher)
        {
        }

        public override async void Activated()
        {
            CancellationToken token = default;
            try
            {
                token = GetCancelationToken();

                await Task.Run(async () => await LoadDataAsync(token)).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                Logger.LoggingInformation("Loading was canceled", this);
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
            }
            finally
            {
                await DispatchToUI(() => ReleaseCancelationToken(token)).ConfigureAwait(false);
            }
        }

        protected abstract Task LoadDataAsync(CancellationToken token);

        protected async Task EnsureAccesTokenAsync(ISessionService sessionService)
        {
            try
            {
                await sessionService.EnsureAccessTokenNotExpiredAsync().ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LoggingException(this, ex);
                await DispatchToUI(() => NavigationService.ShowModalAsync(RouteNames.LoginPage)).ConfigureAwait(false);
            }
        }
    }
}
