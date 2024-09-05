namespace Workout.Planner.ViewModels
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Services;

    public abstract class BaseViewModel : INotifyPropertyChanged, IActiveAware
    {
        private readonly INavigationService _navigationService;
        private readonly ILogger<BaseViewModel> _logger;
        private readonly IDispatcher _dispatcher;

        public BaseViewModel(INavigationService navigationService, ILogger<BaseViewModel> logger, IDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(dispatcher);
            ArgumentNullException.ThrowIfNull(navigationService);
            _navigationService = navigationService;
            _logger = logger;
            _dispatcher = dispatcher;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected INavigationService NavigationService
        {
            get { return _navigationService; }
        }

        protected ILogger<BaseViewModel> Logger
        {
            get { return _logger; }
        }

        public virtual void Activated()
        {
        }

        public virtual void Deactivated()
        {
        }

        protected async Task DispatchToUI(Action action)
        {
            if (_dispatcher.IsDispatchRequired)
            {
                await _dispatcher.DispatchAsync(action);
            }

            action?.Invoke();
        }

        protected async Task DispatchToUI(Func<Task> action)
        {
            if (_dispatcher.IsDispatchRequired)
            {
                await _dispatcher.DispatchAsync(action);
                return;
            }

            await action.Invoke();
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected virtual void RefreshCommands()
        {
        }
    }
}
