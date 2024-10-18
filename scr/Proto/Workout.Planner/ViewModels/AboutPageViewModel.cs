namespace Workout.Planner.ViewModels
{
    using Microsoft.Extensions.Logging;
    using Workout.Planner.Services.Contracts;
    using Workout.Planner.Strings;

    public class AboutPageViewModel : BaseViewModel
    {
        public AboutPageViewModel(
            INavigationService navigationService,
            ILogger<AboutPageViewModel> logger,
            IDispatcher dispatcher)
            : base(navigationService, logger, dispatcher)
        {
        }

        // TODO wie bekommt man eine version....
        public string Version => AppStrings.Version;

        public string Developer => AppStrings.Developer;

        protected override string? Validate(string collumName)
        {
            return string.Empty;
        }
    }
}
