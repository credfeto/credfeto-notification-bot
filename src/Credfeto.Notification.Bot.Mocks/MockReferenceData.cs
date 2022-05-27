using Credfeto.Notification.Bot.Mocks.Objects;
using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks;

public static class MockReferenceData
{
    public static MockBase<TwitchAuthentication> TwitchAuthentication { get; } = new MockTwitchAuthentication();
}