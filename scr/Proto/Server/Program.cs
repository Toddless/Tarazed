using Microsoft.AspNetCore;
using Serilog;
using Serilog.Events;
using Server;

public class Program
{
    public static void Main(string[] args)
    {
       BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
      .WriteTo.Console()
      .WriteTo.File(
          "logs/log.txt",
          rollingInterval: RollingInterval.Day,
          rollOnFileSizeLimit: true,
          fileSizeLimitBytes: 1024 * 1024 * 1024)
      .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Debug)
      .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Debug)
      .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Debug)
      .CreateLogger();

        var builder = WebHost.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostContext, config) =>
             {
                 // delete all default configuration providers
                 // config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                 // config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
             });
        return builder!
            .ConfigureLogging(x => x.AddSerilog())
            .UseStartup<Startup>()
            .Build();
    }
}
