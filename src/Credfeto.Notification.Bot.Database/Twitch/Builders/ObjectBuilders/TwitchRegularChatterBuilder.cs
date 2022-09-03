using System.Data;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchRegularChatterBuilder : IObjectBuilder<TwitchRegularChatterEntity, TwitchRegularChatter>
{
    public TwitchRegularChatter Build(TwitchRegularChatterEntity source)
    {
        return new(source.Chat_User ?? throw new DataException("Unknown Chat_User"), regular: source.Regular);
    }
}