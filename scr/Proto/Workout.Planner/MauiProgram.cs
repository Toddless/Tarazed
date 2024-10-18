namespace Workout.Planner
{
    using Microsoft.Extensions.Logging;
    using Serilog;
    using SkiaSharp.Views.Maui.Controls.Hosting;
    using Workout.Planner.Services;
    using Workout.Planner.Services.Contracts;
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
                .UseSkiaSharp()
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
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<ITrainingService, TrainingService>();
            builder.Services.AddSingleton<IExerciseService, ExerciseService>();
            builder.Services.AddSingleton<IUnitService, UnitService>();
            builder.Services.AddTransient<UserPage>();
            builder.Services.AddTransient<UnitPage>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<AboutPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ExercisePage>();
            builder.Services.AddTransient<EditUnitPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<RegisterUserPage>();
            builder.Services.AddTransient<PasswordRecoveryPage>();
            builder.Services.AddTransient<EditTrainingPage>();
            builder.Services.AddTransient<EditExercisePage>();
            builder.Services.AddTransient<HomePageViewModel>();
            builder.Services.AddTransient<UserPageViewModel>();
            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<UnitPageViewModel>();
            builder.Services.AddTransient<LoginPageViewModel>();
            builder.Services.AddTransient<AboutPageViewModel>();
            builder.Services.AddTransient<ExercisePageViewModel>();
            builder.Services.AddTransient<EditUnitPageViewModel>();
            builder.Services.AddTransient<SettingsPageViewModel>();
            builder.Services.AddTransient<EditExercisePageViewModel>();
            builder.Services.AddTransient<RegisterUserPageViewModel>();
            builder.Services.AddTransient<EditTrainingPageViewModel>();
            builder.Services.AddTransient<PasswordRecoveryPageViewModel>();

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
