using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[DebuggerDisplay("Chat: {ChatWelcomesEnabled} Raid: {RaidWelcomesEnabled} Thanks: {ThanksEnabled} Shoutouts: {ShoutOutsEnabled} Milestones: {AnnounceMilestonesEnabled}")]
public sealed class StreamSettings
{
    public StreamSettings(bool chatWelcomesEnabled, bool raidWelcomesEnabled, bool thanksEnabled, bool announceMilestonesEnabled, bool shoutOutsEnabled)
    {
        this.ChatWelcomesEnabled = chatWelcomesEnabled;
        this.RaidWelcomesEnabled = raidWelcomesEnabled;
        this.ThanksEnabled = thanksEnabled;
        this.AnnounceMilestonesEnabled = announceMilestonesEnabled;
        this.ShoutOutsEnabled = shoutOutsEnabled;
    }

    public bool ChatWelcomesEnabled { get; }

    public bool RaidWelcomesEnabled { get; }

    public bool ThanksEnabled { get; }

    public bool AnnounceMilestonesEnabled { get; }

    public bool ShoutOutsEnabled { get; }
}