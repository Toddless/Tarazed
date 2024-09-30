namespace Workout.Planner.ViewModels
{
    using System.ComponentModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;

    public abstract class BaseViewModel : ObservableObject, IActiveAware, IDataErrorInfo
    {
        private readonly INavigationService _navigationService;
        private readonly ILogger<BaseViewModel> _logger;
        private readonly IDispatcher _dispatcher;
        private readonly List<string> _fieldsToVlaidate = [];
        private CancellationTokenSource? _cts;

        public BaseViewModel(
            INavigationService navigationService,
            ILogger<BaseViewModel> logger,
            IDispatcher dispatcher)
        {
            ArgumentNullException.ThrowIfNull(navigationService);
            ArgumentNullException.ThrowIfNull(dispatcher);
            ArgumentNullException.ThrowIfNull(logger);
            _cts = new CancellationTokenSource();
            _navigationService = navigationService;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        public string Error
        {
            get
            {
                return string.Join(",", _fieldsToVlaidate.Select(x => this[x]).Where(x => !string.IsNullOrWhiteSpace(x)));
            }
        }

        public bool IsBusy
        {
            get => CancellationTokenSources.Any();
        }

        protected bool HasError => !string.IsNullOrWhiteSpace(Error);

        protected CancellationTokenSource Cts
        {
            get
            {
                _cts ??= new CancellationTokenSource();

                return _cts;
            }
        }

        protected IList<CancellationTokenSource> CancellationTokenSources { get; } = [];

        protected INavigationService NavigationService
        {
            get { return _navigationService; }
        }

        protected ILogger<BaseViewModel> Logger
        {
            get { return _logger; }
        }

        public string this[string collumName]
        {
            get
            {
                // null ist nur dann möglich, wenn die methode "RegisterProperty" in der classe
                // ohne [PropertyToValide] attribute und dann indexer aufgeruft wurde
                return Validate(collumName) !;
            }
        }

        public virtual void Activated()
        {
        }

        public virtual void Deactivated()
        {
            try
            {
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

        protected void RegisterProperties()
        {
            var properties = GetType().GetProperties().Where(x => x.GetCustomAttributes(typeof(PropertyToValidateAttribute), false).Length > 0);

            foreach (var property in properties)
            {
                _fieldsToVlaidate.Add(property.Name);
            }
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

        protected async Task DispatchToUI(Action action)
        {
            if (_dispatcher.IsDispatchRequired)
            {
                await _dispatcher.DispatchAsync(action);
                return;
            }

            action?.Invoke();
        }

        protected Task DispatchToUIAsync(Func<Task> action)
        {
            return _dispatcher.DispatchToUIAsync(action);
        }

        protected CancellationToken GetCancelationToken()
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(Cts!.Token);
            var token = linkedCts.Token;
            CancellationTokenSources.Add(linkedCts);
            RaisePropertyChanged(nameof(IsBusy));
            return token;
        }

        protected virtual void OnEntryUnfocused(object sender)
        {
            if (sender is not string property)
            {
                return;
            }

            Validate(property);
            RaisePropertyChanged("Item");
            RefreshCommands();
        }

        protected virtual void RefreshCommands()
        {
        }

        protected abstract string? Validate(string collumName);
    }
}
