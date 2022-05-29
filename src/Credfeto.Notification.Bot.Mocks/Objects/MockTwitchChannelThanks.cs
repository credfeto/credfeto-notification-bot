using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchChannelThanks : MockBase<TwitchChannelThanks>
{
    public MockTwitchChannelThanks()
        : base(new(true))
    {
    }

    public override TwitchChannelThanks Next()
    {
        return new(false);
    }
}