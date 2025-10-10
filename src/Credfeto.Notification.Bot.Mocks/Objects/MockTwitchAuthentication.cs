using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockTwitchAuthentication : MockBase<TwitchAuthentication>
{
    private static readonly TwitchAuthentication Invalid = new()
    {
        Api = new()
        {
            ClientId = "INVALID",
            ClientSecret = "INVALID",
            ClientAccessToken = "INVALID",
        },
        Chat = new() { OAuthToken = "INVALID", UserName = "INVALID" },
    };

    public MockTwitchAuthentication()
        : base(Invalid) { }

    public override TwitchAuthentication Next()
    {
        return Invalid;
    }
}
