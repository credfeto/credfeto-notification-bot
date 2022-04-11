using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Services;

public static class Types
{
    public static User UserFromString(string username)
    {
        return new(username.ToLowerInvariant());
    }

    public static Channel ChannelFromString(string username)
    {
        return new(username.ToLowerInvariant());
    }

    public static Channel ChannelFromUser(this in User user)
    {
        return new(user.Value);
    }

    public static User UserFromChannel(this in Channel channel)
    {
        return new(channel.Value);
    }
}