namespace Workout.Planner.ViewModels
{
    using System.Threading.Tasks;
    using DataModel;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services;
    using Workout.Planner.Views;

    public class LoginPageViewModel : BaseViewModel
    {
        private ILoginService _loginService;

        private string? _email = "s@s.s";
        private string? _password = "String1";
        private bool _isBusy;

        public LoginPageViewModel(
            INavigationService navigationService,
            ILoginService loginService,
            ILogger<LoginPageViewModel> logger,
            IDispatcher dispatcher)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(loginService);
            RegisterCommand = new Command(async () => await RegisterUser(), CanLogin);
            LoginCommand = new Command(async () => await LoginUserAsync(), CanLogin);
            _loginService = loginService;
        }

        public Command RegisterCommand { get; }

        public Command LoginCommand { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string? Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    RefreshCommands();
                }
            }
        }

        public string? Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    RefreshCommands();
                }
            }
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            LoginCommand?.ChangeCanExecute();
            RegisterCommand?.ChangeCanExecute();
        }

        private bool CanLogin()
        {
            return !IsBusy && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task RegisterUser()
        {
            try
            {
                IsBusy = true;
                UserRequest user = new ()
                {
                    Email = Email!,
                    Password = Password!,
                };
                await _loginService.RegisterAsync(user).ConfigureAwait(false);

                await DispatchToUI(() => NavigationService.NavigateToAsync(nameof(HomePage)).ConfigureAwait(false));
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

        private async Task LoginUserAsync()
        {
            try
            {
                IsBusy = true;
                UserRequest user = new ()
                {
                    Email = Email!,
                    Password = Password!,
                };
                await _loginService.LoginAsync(user).ConfigureAwait(false);

                await UserLoggedAsync();
            }
            catch (Exception ex)
            {
                Logger.LoggingException(this, ex);
                await DispatchToUI(() => Shell.Current.DisplayAlert("Authentication", ExceptionMessages.IncorrectEmailOrPassword, "Ok")).ConfigureAwait(false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task UserLoggedAsync()
        {
            await DispatchToUI(() => NavigationService.PopPopupPageAsync()).ConfigureAwait(false);
        }
    }
}
