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
        private readonly IService<TEntity> _service;
        private TEntity? _entity;
        private string? _name;
        private long? _id;

        public EditViewModelBase(
            INavigationService navigationService,
            ILogger<EditViewModelBase<TEntity>> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IService<TEntity> service)
            : base(navigationService, logger, dispatcher, sessionService)
        {
            ArgumentNullException.ThrowIfNull(service);
            SaveCommand = new Command(ExecuteSaveChangesAsync, CanSaveChanges);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
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

        protected TEntity? Entity
        {
            get { return _entity; }
        }

        protected IService<TEntity> Service
        {
            get { return _service; }
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
                await EnsureAccesTokenAsync(token).ConfigureAwait(false);

                // default wert fürs long = 0. Statt HasValue sollte man != 0 nutzen
                if (Id != 0)
                {
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
                    var result = await _service.GetDataAsync(false, token, [Id!.Value]).ConfigureAwait(false);
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

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

        protected async void ExecuteSaveChangesAsync()
        {
            CancellationToken token = default;
            try
            {
                token = GetCancelationToken();
                await EnsureAccesTokenAsync(token).ConfigureAwait(false);
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
#pragma warning disable CS8604 // Possible null reference argument.
                    result = ValidationExtensions.ValidateName(Name);
#pragma warning restore CS8604 // Possible null reference argument.
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
