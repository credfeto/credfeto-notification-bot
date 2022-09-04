using System.Data;
using Credfeto.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchViewerUserBuilder : IObjectBuilder<TwitchViewerUserEntity, TwitchUser>
{
    public TwitchUser Build(TwitchViewerUserEntity source)
    {
        return new(source.Id ?? throw new DataException("Missing Id"),
                   Viewer.FromString(source.UserName ?? throw new DataException("Missing Username")),
                   isStreamer: false,
                   dateCreated: source.DateCreated);
    }
}