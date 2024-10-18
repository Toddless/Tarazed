namespace Workout.Planner.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using DataModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class LoginPageViewModel : BaseViewModel
    {
        private ILoginService _loginService;
#if DEBUG
        private string? _password = "String1";
        private string? _email = "mail@mail.com";

#else
        private string _password;
        private string _email;
#endif
        public LoginPageViewModel(
            INavigationService navigationService,
            ILoginService loginService,
            ILogger<LoginPageViewModel> logger,
            IDispatcher dispatcher)
            : base(navigationService, logger, dispatcher)
        {
            ArgumentNullException.ThrowIfNull(loginService);
            RecoveryPasswordCommand = new Command(async () => await ExecutePasswordRecoveryAsync(), CanPasswordRecovery);
            RegisterCommand = new Command(async () => await ExecuteRegisterUser(), CanRegisterUser);
            LoginCommand = new Command(async () => await ExecuteLoginUserAsync(), CanLoginUser);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            _loginService = loginService;
            RegisterProperties();
        }

        public ICommand EntryUnfocusedCommand { get; private set; }

        public Command RecoveryPasswordCommand { get; }

        public Command RegisterCommand { get; }

        public Command LoginCommand { get; }

        [PropertyToValidate]
        public string? Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        [PropertyToValidate]
        public string? Email
        {
            get => _email;
            set { SetProperty(ref _email, value); }
        }

        protected async Task ExecuteLoginUserAsync()
        {
            try
            {
                UserRequest user = new()
                {
                    Email = Email!,
                    Password = Password!,
                };
                await _loginService.LoginAsync(user).ConfigureAwait(false);

                await UserLoggedAsync();
            }
            catch (Exception ex) when (ex is HttpRequestException)
            {
                Logger.LoggingException(this, ex);
                await NavigationService.DisplayAlertOnUiAsync(
                    AppStrings.Authentication,
                    ExceptionMessages.IncorrectEmailOrPassword,
                    AppStrings.OkButton).ConfigureAwait(false);
            }
        }

        protected async Task ExecutePasswordRecoveryAsync()
        {
            await NavigationService.NavigateToOnUIAsync(RouteNames.PasswordRecoveryPage).ConfigureAwait(false);
        }

        protected async Task UserLoggedAsync()
        {
            await NavigationService.CloseModalAsync().ConfigureAwait(false);
        }

        protected async Task ExecuteRegisterUser()
        {
            await NavigationService.NavigateToOnUIAsync(RouteNames.RegisterUserPage).ConfigureAwait(false);
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            LoginCommand?.ChangeCanExecute();
            RegisterCommand?.ChangeCanExecute();
            RecoveryPasswordCommand?.ChangeCanExecute();
        }

        protected override string? Validate(string collumName)
        {
            var result = string.Empty;
            switch (collumName)
            {
                case nameof(Email):
                    result = ValidationExtensions.ValidateEmail(Email);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }

                    break;
                case nameof(Password):
                    result = ValidationExtensions.ValidatePassword(Password);
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

        private bool CanPasswordRecovery()
        {
            return !IsBusy && !HasError;
        }

        private bool CanLoginUser()
        {
            return !IsBusy && !HasError;
        }

        private bool CanRegisterUser()
        {
            return !IsBusy;
        }
    }
}
