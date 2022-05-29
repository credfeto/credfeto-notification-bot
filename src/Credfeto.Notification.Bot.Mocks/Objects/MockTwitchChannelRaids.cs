using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchChannelRaids : MockBase<TwitchChannelRaids>
{
    public MockTwitchChannelRaids()
        : base(new(enabled: false, immediate: null, calmDown: null))
    {
    }

    public override TwitchChannelRaids Next()
    {
        return new(enabled: false,
                   new[]
                   {
                       "!raiders"
                   },
                   new[]
                   {
                       "!tag"
                   });
    }
}