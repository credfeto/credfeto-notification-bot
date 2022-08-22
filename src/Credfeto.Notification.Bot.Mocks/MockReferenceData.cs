using System;
using Credfeto.Notification.Bot.Mocks.Objects;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks;

public static class MockReferenceData
{
    public static MockBase<Streamer> Streamer { get; } = new MockStreamer();

    public static MockBase<Viewer> Viewer { get; } = new MockViewer();

    public static Viewer Ignored { get; } = Viewer.Next();

    public static MockBase<TwitchAuthentication> TwitchAuthentication { get; } = new MockTwitchAuthentication();

    public static TwitchMilestones TwitchMilestones { get; } = new(new[]
                                                                   {
                                                                       1,
                                                                       10,
                                                                       100,
                                                                       1000,
                                                                       2000,
                                                                       3000,
                                                                       4000
                                                                   },
                                                                   Array.Empty<int>());

    public static MockBase<string[]> IgnoredUsers { get; } = new MockIgnoredUsers();

    public static MockBase<string[]> Heists { get; } = new MockHeists();

    public static MockBase<TwitchChannelShoutout> TwitchChannelShoutout { get; } = new MockTwitchChannelShoutout();

    public static MockBase<TwitchChannelRaids> TwitchChannelRaids { get; } = new MockTwitchChannelRaids();

    public static MockBase<TwitchChannelThanks> TwitchChannelThanks { get; } = new MockTwitchChannelThanks();

    public static MockBase<TwitchChannelMileStone> TwitchChanelMileStone { get; } = new MockTwitchChanelMileStone();

    public static MockBase<TwitchChannelWelcome> TwitchChannelWelcome { get; } = new MockTwitchChannelWelcome();
}