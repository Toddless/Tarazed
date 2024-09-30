namespace Workout.Planner.ViewModels
{
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Extensions;
    using Workout.Planner.Services.Contracts;

    public class SettingsPageViewModel : BaseViewModel
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        public SettingsPageViewModel(
            INavigationService navigationService,
            ILogger<SettingsPageViewModel> logger,
            IDispatcher dispatcher,
            ISessionService sessionService,
            IUserService userService)
            : base(navigationService, logger, dispatcher)
        {
            DeleteUserProfileCommand = new Command(DeleteUserProfileAsync);
            ChangeUserEmailCommand = new Command(ChangeUserEmailAsync);
            _sessionService = sessionService;
            _userService = userService;
        }

        public Command DeleteUserProfileCommand { get; }

        public Command ChangeUserEmailCommand { get; }

        protected async void DeleteUserProfileAsync()
        {
            CancellationToken token = default;
            try
            {
                token = GetCancelationToken();
                var userAnswer = await NavigationService.DisplayPromtOnUiAsync(
                    Strings.AppStrings.Warning,
                    // statt delete passwort eingeben
                    // Ich lasse es so, wie es ist, da ich keinen Zugriff auf den
                    // auf der Serverseite verwendeten Passwort-Hashing-Algorithmus habe
                    Strings.AppStrings.DeletingProfile,
                    Strings.AppStrings.OkButton,
                    Strings.AppStrings.CancelButton).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(userAnswer))
                {
                    // user abortet delete
                    return;
                }

                var res = await _userService.DeleteUserAsync(token).ConfigureAwait(false);
                if (res)
                {
                    await NavigationService.DisplayAlertOnUiAsync(
                        Strings.AppStrings.Warning,
                        Strings.AppStrings.Deleted,
                        Strings.AppStrings.OkButton).ConfigureAwait(false);

                    await _sessionService.UserLogoutAsync().ConfigureAwait(false);
                }
                else
                {
                    await NavigationService.DisplayAlertOnUiAsync(
                        Strings.AppStrings.Warning,
                        Strings.AppStrings.SomethingWrong,
                        Strings.AppStrings.OkButton).ConfigureAwait(false);
                    return;
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

        protected override string? Validate(string collumName)
        {
            return string.Empty;
        }

        protected async void ChangeUserEmailAsync()
        {
            await NavigationService.NavigateToOnUIAsync(RouteNames.UserPage).ConfigureAwait(false);
        }
    }
}
