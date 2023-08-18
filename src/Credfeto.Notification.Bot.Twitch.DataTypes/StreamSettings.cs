using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[DebuggerDisplay("Chat: {ChatWelcomesEnabled} Raid: {RaidWelcomesEnabled} Thanks: {ThanksEnabled} Shoutouts: {ShoutOutsEnabled} Milestones: {AnnounceMilestonesEnabled}")]
public sealed record StreamSettings(bool ChatWelcomesEnabled, bool RaidWelcomesEnabled, bool ThanksEnabled, bool AnnounceMilestonesEnabled, bool ShoutOutsEnabled);