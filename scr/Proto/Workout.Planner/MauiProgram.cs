namespace Workout.Planner
{
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Workout.Planner.Services;
    using Workout.Planner.ViewModels;
    using Workout.Planner.Views;

    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            SetupSerilog();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddKeyedSingleton(typeof(IRestApiService), "AuthRestAPI", typeof(RestApiService));
            builder.Services.AddKeyedSingleton(typeof(IRestApiService), "UnAuthRestAPI", typeof(RestApiService));
            builder.Services.AddSingleton<ISessionService, SessionService>();
            builder.Services.AddSingleton<ILoginService, LoginService>();
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddSingleton<ITrainingService, TrainingService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<EditTrainingPage>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<EditTrainingPageViewModel>();

#if DEBUG
            builder.Logging.AddSerilog(Log.Logger);
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void SetupSerilog()
        {
            var direct = Path.Combine(FileSystem.Current.AppDataDirectory, "log");
            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }

            var file = Path.Combine(direct, "logs.txt");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(
                file,
                flushToDiskInterval: new TimeSpan(0, 0, 1),
                encoding: System.Text.Encoding.UTF8,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1024 * 1024 * 100)
                .CreateLogger();
        }
    }
}
