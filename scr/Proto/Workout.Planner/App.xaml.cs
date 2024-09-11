namespace Workout.Planner
{
    using System.Globalization;

    public partial class App : Application
    {
        public App()
        {
            SetCulture();
            InitializeComponent();
            MainPage = new AppShell();
        }

        private void SetCulture()
        {
            //var culture = new CultureInfo("de-DE");
            //CultureInfo.CurrentUICulture = culture;
            //CultureInfo.DefaultThreadCurrentCulture = culture;
            //CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        protected override void OnStart()
        {
            SetCulture();
            base.OnStart();
        }
    }
}
