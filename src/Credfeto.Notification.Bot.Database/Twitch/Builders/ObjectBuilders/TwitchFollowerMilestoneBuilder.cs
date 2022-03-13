using System.Data;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchFollowerMilestoneBuilder : IObjectBuilder<TwitchFollowerMilestoneEntity, TwitchFollowerMilestone>
{
    public TwitchFollowerMilestone? Build(TwitchFollowerMilestoneEntity? source)
    {
        if (source == null)
        {
            return null;
        }

        return new(source.Channel ?? throw new DataException("Unknown Channel"), followers: source.Followers, freshlyReached: source.Freshly_Reached);
    }
}