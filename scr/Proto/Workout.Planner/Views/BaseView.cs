namespace Workout.Planner.Views
{
    using Workout.Planner.Services.Contracts;

   /// <summary>
   /// Basis view with overrided OnApperaring and OnDisappearing.
   /// </summary>
    public partial class BaseView : ContentPage
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
