namespace Workout.Planner.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using DataModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Microsoft.Maui.Controls;
    using Workout.Planner.Extensions;
    using Workout.Planner.Strings;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Helper;

    public class RegisterUserPageViewModel : BaseViewModel
    {
        private readonly ILoginService _loginService;
        private string _confirmPassword;
        private string _password;
        private string _email;

        public RegisterUserPageViewModel(
            INavigationService navigationService,
            ILogger<RegisterUserPageViewModel> logger,
            IDispatcher dispatcher,
            ILoginService loginService)
            : base(navigationService, logger, dispatcher)
        {
            RegisterUserPageBackButtonCommand = new Command(RegisterUserPageBackButtonAsync);
            CreateProfileCommand = new Command(async () => await CreateUserProfileAsync(), CanCreate);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            _loginService = loginService;
            RegisterProperties();
        }

        public ICommand EntryUnfocusedCommand { get; private set; }

        public Command RegisterUserPageBackButtonCommand { get; }

        public Command CreateProfileCommand { get; }

        [PropertyToValidate]
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { SetProperty(ref _confirmPassword, value); }
        }

        [PropertyToValidate]
        public string Email
        {
            get => _email;
            set { SetProperty(ref _email, value); }
        }

        [PropertyToValidate]
        public string Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        public async void RegisterUserPageBackButtonAsync()
        {
            await NavigationService.ShowModalAsync(RouteNames.LoginPage).ConfigureAwait(false);
        }

        public async Task CreateUserProfileAsync()
        {
            try
            {
                UserRequest register = new() { Email = Email, Password = Password };

                await _loginService.RegisterAsync(register).ConfigureAwait(false);

                await _loginService.LoginAsync(register).ConfigureAwait(false);

                await NavigationService.CloseModalAsync().ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LoggingException(this, ex);
                await NavigationService.DisplayAlertOnUiAsync(
                      AppStrings.Information,
                      AppStrings.EmailAlreadyExists,
                      AppStrings.OkButton).ConfigureAwait(false);
            }
        }

        protected override string? Validate(string propertyToValidate)
        {
            string result = string.Empty;
            switch (propertyToValidate)
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

                case nameof(ConfirmPassword):
                    if (Password != ConfirmPassword)
                    {
                        return AppStrings.PasswordNotMatch;
                    }

                    break;
                default:
                    return AppStrings.SomethingWrong;
            }

            return null;
        }

        protected override void RefreshCommands()
        {
            base.RefreshCommands();
            CreateProfileCommand?.ChangeCanExecute();
        }

        private bool CanCreate()
        {
            return !HasError && !IsBusy;
        }
    }
}
