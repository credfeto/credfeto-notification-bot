using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchChannelWelcome : MockBase<TwitchChannelWelcome>
{
    public MockTwitchChannelWelcome()
        : base(new(true))
    {
    }

    public override TwitchChannelWelcome Next()
    {
        return new(false);
    }
}