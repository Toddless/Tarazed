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
        private Customer? _customer;
        private string? _email;

        public UserPageViewModel(
            INavigationService navigationService,
            ILogger<UserPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IUserService userService)
            : base(navigationService, logger, dispatcher, sessionService)
        {
            SaveChangesCommand = new Command(async () => await ExecuteSaveChangesAsync());
            ArgumentNullException.ThrowIfNull(userService);
            _userService = userService;
        }

        public Customer? Customer
        {
            get => _customer;

            // customer liegt im hintergrund und wird nie an der Oberfläche gezeigt =>
            // SetProperty nicht benötigt
            private set => _customer = value;
        }

        public Command SaveChangesCommand { get; }

        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public async Task ExecuteSaveChangesAsync()
        {
            CancellationToken token = default;
            try
            {
                // userpage noch nicht komplett bearbeitet. Erste Entwurf hatte ich schon gemacht
                // aber im großen und ganzen noch keine gedanken.
                token = GetCancelationToken();
                await EnsureAccesTokenAsync().ConfigureAwait(false);
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
                await EnsureAccesTokenAsync().ConfigureAwait(false);

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
                        return result;
                    }

                    break;

                default:
                    return AppStrings.PropertyToValidateNotFound;
            }

            return null;
        }
    }
}
