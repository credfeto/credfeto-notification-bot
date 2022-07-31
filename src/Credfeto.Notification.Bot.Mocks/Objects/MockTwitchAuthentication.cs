using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchAuthentication : MockBase<TwitchAuthentication>
{
    private static readonly TwitchAuthentication Invalid = new(oAuthToken: "INVALID",
                                                               userName: "INVALID",
                                                               clientId: "INVALID",
                                                               clientSecret: "INVALID",
                                                               clientAccessToken: "INVALID");

    public MockTwitchAuthentication()
        : base(Invalid)
    {
    }

    public override TwitchAuthentication Next()
    {
        return Invalid;
    }
}