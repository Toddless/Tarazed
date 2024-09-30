namespace Workout.Planner.ViewModels
{
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services.Contracts;

    public abstract class LoadDataBaseViewModel : BaseViewModel
    {
        private CancellationTokenSource? _cts;

        protected LoadDataBaseViewModel(INavigationService navigationService, ILogger<LoadDataBaseViewModel> logger, IDispatcher dispatcher)
            : base(navigationService, logger, dispatcher)
        {
            _cts = new CancellationTokenSource();
        }

        public bool IsBusy
        {
            get => CancellationTokenSources.Any();
        }

        protected IList<CancellationTokenSource> CancellationTokenSources { get; } = [];

        public override async void Activated()
        {
            base.Activated();
            CancellationToken token = default;
            try
            {
                _cts ??= new CancellationTokenSource();

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

        public override void Deactivated()
        {
            try
            {
                base.Deactivated();
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
                CancellationTokenSources.Clear();
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
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

        protected CancellationToken GetCancelationToken()
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts!.Token);
            var token = linkedCts.Token;
            CancellationTokenSources.Add(linkedCts);
            RaisePropertyChanged(nameof(IsBusy));
            return token;
        }

        protected void ReleaseCancelationToken(CancellationToken token)
        {
            var source = CancellationTokenSources.FirstOrDefault(x => x.Token == token);
            if (source != null)
            {
                CancellationTokenSources.Remove(source);
            }

            RaisePropertyChanged(nameof(IsBusy));

            if (!IsBusy)
            {
                RefreshCommands();
            }
        }
    }
}
