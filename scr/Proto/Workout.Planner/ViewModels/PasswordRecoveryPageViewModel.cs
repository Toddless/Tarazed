namespace Workout.Planner.ViewModels
{
    using System.Threading;
    using System.Windows.Input;
    using DataModel;
    using DataModel.Attributes;
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Helper;
    using Workout.Planner.Models;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class PasswordRecoveryPageViewModel : LoadDataBaseViewModel
    {
        private readonly ILoginService _loginService;
        private string? _confirmPassword;
        private string? _newPassword;
        private string? _resetCode;
        private string? _email;
        private bool _emailIsSent;

        public PasswordRecoveryPageViewModel(
            INavigationService navigationService,
            ILogger<PasswordRecoveryPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            ILoginService loginService)
            : base(navigationService, logger, dispatcher, sessionService)
        {
            SaveNewPasswordCommand = new Command(async () => await ExecuteSaveNewPasswordAsync(), CanSaveNewPassword);
            EntryUnfocusedCommand = new Command(OnEntryUnfocused);
            BackCommand = new Command(ExecuteBackAsync);
            _loginService = loginService;
            RegisterProperties();
        }

        public ICommand EntryUnfocusedCommand { get; private set; }

        public Command BackCommand { get; }

        public Command SaveNewPasswordCommand { get; }

        public bool EmailIsSent
        {
            get => _emailIsSent;
            set { SetProperty(ref _emailIsSent, value); }
        }

        [PropertyToValidate]
        public string? ResetCode
        {
            get => _resetCode;
            set { SetProperty(ref _resetCode, value); }
        }

        [PropertyToValidate]
        public string? ConfirmPassword
        {
            get => _confirmPassword;
            set { SetProperty(ref _confirmPassword, value); }
        }

        [PropertyToValidate]
        public string? NewPassword
        {
            get => _newPassword;
            set { SetProperty(ref _newPassword, value); }
        }

        [PropertyToValidate]
        public string? Email
        {
            get => _email;
            set { SetProperty(ref _email, value); }
        }

        protected override async Task LoadDataAsync(CancellationToken token)
        {
            try
            {
                GetCancelationToken();
                token.ThrowIfCancellationRequested();

                // todo: popups loswerden
                string answer = await NavigationService.DisplayPromtOnUiAsync(
                    AppStrings.Information,
                    AppStrings.EnterEmailToPasswordRecovery,
                    AppStrings.Send,
                    AppStrings.CancelButton).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(answer))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(ValidationExtensions.ValidateEmail(answer)))
                {
                    EmailIsSent = true;
                    Email = answer;
                    PasswordRecoveryModel passwordRecovery = new() { Email = answer };
                    await _loginService.RecoverUserPasswordAsync(passwordRecovery, true);

                    // todo: popups loswerden
                    await NavigationService.DisplayAlertOnUiAsync(
                        AppStrings.Information,
                        AppStrings.ResetCodeIsSend,
                        AppStrings.OkButton).ConfigureAwait(false);
                }
                else
                {
                    // todo: popups loswerden
                    await NavigationService.DisplayAlertOnUiAsync(
                        AppStrings.Warning,
                        AppStrings.EmailFormatWrong,
                        AppStrings.OkButton).ConfigureAwait(false);
                    await LoadDataAsync(token);
                }
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

        protected async Task ExecuteSaveNewPasswordAsync()
        {
            try
            {
                PasswordRecoveryModel recovery = new() { Email = Email, ResetCode = ResetCode, NewPassword = NewPassword };

                // todo: das feedback fällt, nicht ganz klar ob das request erfolgreich war oder nicht
                await _loginService.RecoverUserPasswordAsync(recovery, false).ConfigureAwait(false);

                // todo: zwei objekte mit den selben werten zu erstellen gefällt mir nicht, mal gucken ob man besser lösen kann
                UserRequest loginAfterRecovery = new() { Email = recovery.Email, Password = recovery.NewPassword };

                await _loginService.LoginAsync(loginAfterRecovery).ConfigureAwait(false);

                await NavigationService.CloseModalAsync().ConfigureAwait(false);
            }
            catch (FormatException ex)
            {
                Logger.LoggingException(this, ex);

                // todo: popups loswerden
                await NavigationService.DisplayAlertOnUiAsync(
                    AppStrings.Information,
                    AppStrings.EmailOrResetCode,
                    AppStrings.OkButton).ConfigureAwait(false);
            }
        }

        protected override string? Validate(string collumName)
        {
            string result = string.Empty;
            switch (collumName)
            {
                case nameof(Email):

                    // Obwohl Email-Feld aufgefühlt ist, erscheint sich das error "IsRequered".
                    // herausfinden wie man das löst.

                    // mögliche null argument handling ist in der methode vorgesehen.
#pragma warning disable CS8604 // Possible null reference argument.
                    result = ValidationExtensions.ValidateEmail(Email);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }

                    break;
                case nameof(NewPassword):
                    result = ValidationExtensions.ValidatePassword(NewPassword);
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
#pragma warning restore CS8604 // Possible null reference argument.

                    break;
                case nameof(ConfirmPassword):
                    if (NewPassword != ConfirmPassword)
                    {
                        result = AppStrings.PasswordNotMatch;
                        return result;
                    }

                    break;
                case nameof(ResetCode):
                    if (string.IsNullOrWhiteSpace(ResetCode))
                    {
                        return AppStrings.IsRequerd;
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
            SaveNewPasswordCommand?.ChangeCanExecute();
        }

        protected async void ExecuteBackAsync()
        {
            await NavigationService.ShowModalAsync(RouteNames.LoginPage).ConfigureAwait(false);
        }

        private bool CanSaveNewPassword()
        {
            return !HasError && !IsBusy;
        }
    }
}
