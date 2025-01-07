namespace Workout.Planner.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Input;
    using DataModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Microsoft.Maui.Controls;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class RegisterUserPageViewModel : BaseViewModel
    {
        private readonly ILoginService _loginService;
        private string? _confirmPassword;
        private string? _password;
        private string? _email;

        public RegisterUserPageViewModel(
            INavigationService navigationService,
            ILogger<RegisterUserPageViewModel> logger,
            IDispatcher dispatcher,
            ILoginService loginService)
            : base(navigationService, logger, dispatcher)
        {
            BackButtonCommand = new Command(ExecuteBackButtonAsync, CanBackButton);
            CreateProfileCommand = new Command(async () => await ExecuteCreateUserProfileAsync(), CanCreateUserProfile);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            _loginService = loginService;
            RegisterProperties();
        }

        public ICommand EntryUnfocusedCommand { get; private set; }

        public Command BackButtonCommand { get; }

        public Command CreateProfileCommand { get; }

        [PropertyToValidate]
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set { SetProperty(ref _confirmPassword, value); }
        }

        [PropertyToValidate]
        public string? Email
        {
            get => _email;
            set { SetProperty(ref _email, value); }
        }

        [PropertyToValidate]
        public string? Password
        {
            get => _password;
            set { SetProperty(ref _password, value); }
        }

        public async void ExecuteBackButtonAsync()
        {
            await NavigationService.ShowModalAsync(RouteNames.LoginPage).ConfigureAwait(false);
        }

        public async Task ExecuteCreateUserProfileAsync()
        {
            try
            {
                UserRequest register = new() { Email = Email, Password = Password };

                // todo: das feedback fällt, nicht ganz klar ob das request erfolgreich war oder nicht
                await _loginService.RegisterAsync(register).ConfigureAwait(false);

                await _loginService.LoginAsync(register).ConfigureAwait(false);

                await NavigationService.CloseModalAsync().ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LoggingException(this, ex);

                // todo: popups loswerden
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
                    // die methode kümmert sich um mögliche null, das als argument übergeben werden kann.
#pragma warning disable CS8604 // Possible null reference argument.
                    result = ValidationExtensions.ValidateEmail(Email);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }

                    break;

                case nameof(Password):
                    result = ValidationExtensions.ValidatePassword(Password);
#pragma warning restore CS8604 // Possible null reference argument.
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
            BackButtonCommand?.ChangeCanExecute();
        }

        private bool CanCreateUserProfile()
        {
            return !IsBusy && !HasError;
        }

        private bool CanBackButton()
        {
            return !IsBusy;
        }
    }
}
