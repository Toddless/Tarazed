namespace Workout.Planner.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using DataModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public abstract class EditViewModelBase<TEntity> : LoadDataBaseViewModel, IQueryAttributable
        where TEntity : class, IHaveName, IEntity, new()
    {
        protected readonly ISessionService _sessionService;
        protected readonly IService<TEntity> _service;
        protected TEntity? _entity;
        private string? _name;
        private long? _id;

        public EditViewModelBase(
            INavigationService navigationService,
            ILogger<EditViewModelBase<TEntity>> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IService<TEntity> service)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(sessionService);
            ArgumentNullException.ThrowIfNull(service);
            SaveCommand = new Command(SaveChangesAsync, CanSaveChanges);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            _sessionService = sessionService;
            _service = service;
            RegisterProperties();
        }

        public ICommand EntryUnfocusedCommand { get; private set; }

        public Command SaveCommand { get; }

        [PropertyToValidate]
        public string? Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    RefreshCommands();
                }
            }
        }

        public string Titel
        {
            get { return Id != 0 ? AppStrings.TitelEdit + " " + EntityName : AppStrings.TitelCreate + " " + EntityName; }
        }

        protected abstract string EntityName { get; }

        protected long? Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged(nameof(Titel));
                }
            }
        }

        protected long? RelatedId { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Id = query.GetValue<long>(NavigationParameterNames.EntityId);
            RelatedId = query.GetValue<long>(NavigationParameterNames.RelatedId);
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);

                // aber im fall wenn es nichts ausgewählt wurde, wird hier null übergeben, was führt dazu, dass
                // wir alle pläne laden. Diese seite kann sich mit mehrere plane nicht umgehen => wird exception geworfen.
                if (Id != 0)
                {
                    var result = await _service.GetDataAsync(false, token, [Id!.Value]).ConfigureAwait(false);

                    _entity = result.Single();
                    await DispatchToUI(() =>
                    {
                        LoadOnUI(_entity);
                    }).ConfigureAwait(false);
                }
                else
                {
                    _entity = new TEntity();
                    await DispatchToUI(() =>
                    {
                        LoadOnUI(_entity);
                    }).ConfigureAwait(false);
                    return;
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException or ObjectDisposedException)
            {
                Logger.LoggingInformation("Loading was canceled", this);
            }
            finally
            {
                await DispatchToUI(() => ReleaseCancelationToken(token)).ConfigureAwait(false);
            }
        }

        protected abstract void LoadOnUI(TEntity entity);

        protected async void SaveChangesAsync()
        {
            CancellationToken token = default;
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);
                token = GetCancelationToken();
                if (string.IsNullOrWhiteSpace(Name))
                {
                    return;
                }

                if (_entity!.Id == default)
                {
                    var result = await _service.CreateDataAsync(GetUpdateEntity(), token).ConfigureAwait(false);
                }
                else
                {
                    await _service.UpdataDataAsync(GetUpdateEntity(), token);
                }

                await NavigationService.NavigateToOnUIAsync(RouteNames.Back).ConfigureAwait(false);
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

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            SaveCommand?.ChangeCanExecute();
        }

        protected abstract TEntity GetUpdateEntity();

        protected override string? Validate(string collumName)
        {
            string result = string.Empty;
            switch (collumName)
            {
                case nameof(Name):
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        return AppStrings.IsRequerd;
                    }

                    result = ValidationExtensions.ValidateName(Name);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }

                    break;
                default:
                    return AppStrings.SomethingWrong;
            }

            return null;
        }

        protected virtual bool CanSaveChanges()
        {
            return !IsBusy && !HasError;
        }
    }
}
