using System.Data;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchFollowerBuilder : IObjectBuilder<TwitchFollowerEntity, TwitchFollower>
{
    public TwitchFollower? Build(TwitchFollowerEntity source)
    {
        return new(source.Channel ?? throw new DataException("Unknown Channel"),
                   source.Follower ?? throw new DataException("Unknown Follower"),
                   followCount: source.Follow_Count,
                   freshlyReached: source.Freshly_Reached);
    }
}