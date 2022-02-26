using Credfeto.Notification.Bot.Twitch.Resources;
using Credfeto.Notification.Bot.Twitch.Resources.Services;
using Credfeto.Notification.Bot.Twitch.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Configures Twitch Integration
/// </summary>
public static class TwitchSetup
{
    /// <summary>
    ///     Configures twitch integration.
    /// </summary>
    /// <param name="services"></param>
    public static void Configure(IServiceCollection services)
    {
        services.AddSingleton<ICurrentTimeSource, CurrentTimeSource>();

        services.AddSingleton<ITwitchChat, TwitchChat>();
    }
}