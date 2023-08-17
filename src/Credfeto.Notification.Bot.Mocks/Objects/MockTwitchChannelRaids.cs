using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchChannelRaids : MockBase<TwitchChannelRaids>
{
    private static readonly string[] ImmediateCommands =
    {
        "!raiders"
    };

    private static readonly string[] CalmDownCommands =
    {
        "!tag"
    };

    public MockTwitchChannelRaids()
        : base(new(enabled: false, immediate: null, calmDown: null))
    {
    }

    public override TwitchChannelRaids Next()
    {
        return new(enabled: false, immediate: ImmediateCommands, calmDown: CalmDownCommands);
    }
}