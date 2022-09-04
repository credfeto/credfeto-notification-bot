using Credfeto.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchStreamSettingsBuilder : IObjectBuilder<StreamSettingsEntity, StreamSettings>
{
    public StreamSettings Build(StreamSettingsEntity source)
    {
        return new(chatWelcomesEnabled: source.Chat_Welcomes,
                   raidWelcomesEnabled: source.Raid_Welcomes,
                   thanksEnabled: source.Thanks,
                   announceMilestonesEnabled: source.Announce_Milestones,
                   shoutOutsEnabled: source.Shout_Outs);
    }
}