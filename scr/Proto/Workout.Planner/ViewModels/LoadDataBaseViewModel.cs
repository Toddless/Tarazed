namespace Workout.Planner.ViewModels
{
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services;

    public abstract class LoadDataBaseViewModel : BaseViewModel
    {
        private bool _isBusy;

        private CancellationTokenSource? _cts;

        protected LoadDataBaseViewModel(INavigationService navigationService, ILogger<LoadDataBaseViewModel> logger, IDispatcher dispatcher)
            : base(navigationService, logger, dispatcher)
        {
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    RefreshCommands();
                }
            }
        }

        public override async void Activated()
        {
            base.Activated();
            try
            {
                if (_cts != null)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                }

                IsBusy = true;
                _cts = new CancellationTokenSource();
                await Task.Run(async () => await LoadDataAsync(_cts.Token)).ConfigureAwait(false);
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
                IsBusy = false;
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
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
            }
        }

        protected abstract Task LoadDataAsync(CancellationToken token);
    }
}
