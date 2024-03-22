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
        TwitchFriendChannel fc1 = new(channel: "Friend1", message: "!friend1");
        TwitchFriendChannel fc2 = new(channel: "Friend2", message: null);

        return new(enabled: true, [fc1, fc2]);
    }
}