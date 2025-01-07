namespace Workout.Planner.Views
{
    using Workout.Planner.Services.Contracts;

    public class BaseView : ContentPage
    {
        private readonly IActiveAware _viewModel;

        public BaseView(IActiveAware viewModel)
        {
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.Activated();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.Deactivated();
        }
    }
}
