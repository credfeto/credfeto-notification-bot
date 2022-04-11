using System.Data;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchUserBuilder : IObjectBuilder<TwitchUserEntity, TwitchUser>
{
    public TwitchUser? Build(TwitchUserEntity? source)
    {
        if (source == null)
        {
            return null;
        }

        return new(source.Id ?? throw new DataException("Missing Id"),
                   Viewer.FromString(source.UserName ?? throw new DataException("Missing Username")),
                   isStreamer: true,
                   dateCreated: source.DateCreated);
    }
}