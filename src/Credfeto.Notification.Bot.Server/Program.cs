using System.Threading.Tasks;
using Credfeto.Notification.Bot.Server.Helpers;
using Credfeto.Notification.Bot.Server.ServiceStartup;
using Microsoft.Extensions.Hosting;

namespace Credfeto.Notification.Bot.Server;

internal static class Program
{
    public static Task Main(string[] args)
    {
        StartupBanner.Show();

        return CreateHostBuilder(args)
               .Build()
               .RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureServices(Services.Configure)
                   .UseWindowsService()
                   .UseSystemd();
    }
}