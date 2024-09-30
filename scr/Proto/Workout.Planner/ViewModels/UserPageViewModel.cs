namespace Workout.Planner.ViewModels
{
    using System.Threading;
    using System.Threading.Tasks;
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class UserPageViewModel : LoadDataBaseViewModel
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        private string? _email;
        private bool _emailIsValid;
        private Customer? _customer;

        public UserPageViewModel(
            INavigationService navigationService,
            ILogger<UserPageViewModel> logger,
            IDispatcher dispatcher,
            IUserService userService,
            ISessionService sessionService)
            : base(navigationService, logger, dispatcher)
        {
            _userService = userService;
            _sessionService = sessionService;
            SaveChangesCommand = new Command(async () => await SaveChangesAsync());
        }

        public Customer? Customer
        {
            get => _customer;
            private set => _customer = value;
        }

        public Command SaveChangesCommand { get; }

        public bool EmailIsValid
        {
            get => _emailIsValid;
            set
            {
                if (SetProperty(ref _emailIsValid, value))
                {
                    RefreshCommands();
                }
            }
        }

        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public async Task SaveChangesAsync()
        {
            CancellationToken token = default;
            try
            {
                token = GetCancelationToken();
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);
                Customer changedUser = new() { Email = Email!, UId = Customer!.UId };
                await _userService.UpdateCustomerAsync(token, changedUser).ConfigureAwait(false);
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

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                await EnsureAccesTokenAsync(_sessionService).ConfigureAwait(false);

                token.ThrowIfCancellationRequested();

                var user = await _userService.GetCurrentUser(token);

                await DispatchToUI(() =>
                {
                    Customer = user;
                    Email = user.Email;
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
            }
        }

        protected override string? Validate(string collumName)
        {
            string result = string.Empty;
            switch (collumName)
            {
                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email))
                    {
                        break;
                    }

                    result = ValidationExtensions.ValidateEmail(Email);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        EmailIsValid = false;
                        return result;
                    }

                    EmailIsValid = true;
                    break;

                default:
                    return AppStrings.PropertyToValidateNotFound;
            }

            return null;
        }
    }
}
