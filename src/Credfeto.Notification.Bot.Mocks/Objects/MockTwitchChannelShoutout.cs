using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchChannelShoutout : MockBase<TwitchChannelShoutout>
{
    public MockTwitchChannelShoutout()
        : base(new(enabled: false, new()))
    {
    }

    public override TwitchChannelShoutout Next()
    {
        return new(enabled: true, new() { new(channel: "Friend1", message: "!friend1"), new(channel: "Friend2", message: null) });
    }
}