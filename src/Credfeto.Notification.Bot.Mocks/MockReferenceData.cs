using Credfeto.Notification.Bot.Mocks.Objects;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks;

public static class MockReferenceData
{
    public static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    public static readonly Viewer Ignored = Viewer.FromString(nameof(Ignored));

    public static MockBase<TwitchAuthentication> TwitchAuthentication { get; } = new MockTwitchAuthentication();

    public static TwitchMilestones TwitchMilestones { get; } = new(new()
                                                                   {
                                                                       1,
                                                                       10,
                                                                       100,
                                                                       1000,
                                                                       2000,
                                                                       3000,
                                                                       4000
                                                                   },
                                                                   new());

    public static List<string> IgnoredUsers { get; } = new();

    public static List<string> Heists { get; } = new();
}