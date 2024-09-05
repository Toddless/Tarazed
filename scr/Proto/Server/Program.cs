using System.Reflection;
using Microsoft.AspNetCore;
using Serilog;
using Server;

public class Program
{
    public static void Main(string[] args)
    {
       BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        var builder = WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostContext, config) =>
             {
                 // delete all default configuration providers
                 // config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                 // config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
             });
        return builder!
            .UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .ConfigureLogging(SetupLogging)
            .UseStartup<Startup>()
            .Build();
    }

    private static void SetupLogging(WebHostBuilderContext context, ILoggingBuilder builder)
    {
        var path = Path.Combine(context.HostingEnvironment.ContentRootPath, "logs", "log.txt");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
#if DEBUG
     .WriteTo.Console()
     .WriteTo.Debug()
#endif
     .WriteTo.File(
         path,
         restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
         rollingInterval: RollingInterval.Day,
         rollOnFileSizeLimit: true,
         fileSizeLimitBytes: 1024 * 1024 * 100)
     .CreateLogger();

        builder.AddSerilog();
    }
}
