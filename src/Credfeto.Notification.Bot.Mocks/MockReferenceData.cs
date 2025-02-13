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

    public static MockBase<TwitchAuthentication> TwitchAuthentication { get; } =
        new MockTwitchAuthentication();
}
