using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchChanelMileStone : MockBase<TwitchChannelMileStone>
{
    public MockTwitchChanelMileStone()
        : base(new(true))
    {
    }

    public override TwitchChannelMileStone Next()
    {
        return new(false);
    }
}