namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchAuthentication
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string OAuthToken { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string UserName { get; init; } = default!;
}