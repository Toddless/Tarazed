namespace Workout.Planner.ViewModels
{
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Services.Contracts;

    public abstract class LoadDataBaseViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;

        protected LoadDataBaseViewModel(
            INavigationService navigationService,
            ILogger<LoadDataBaseViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(sessionService);
            _sessionService = sessionService;
        }

        protected ISessionService SessionService
        {
            get { return _sessionService; }
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

        protected async Task EnsureAccesTokenAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await _sessionService.EnsureAccessTokenNotExpiredAsync(token).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LoggingException(this, ex);
                await NavigationService.ShowModalAsync(RouteNames.LoginPage).ConfigureAwait(false);
            }
        }
    }
}
