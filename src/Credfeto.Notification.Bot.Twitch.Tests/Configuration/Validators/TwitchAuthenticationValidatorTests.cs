using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Configuration.Validators;
using FluentValidation.Results;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Configuration.Validators;

public sealed class TwitchAuthenticationValidatorTests : TestBase
{
    private readonly TwitchAuthenticationValidator _validator;

    public TwitchAuthenticationValidatorTests()
    {
        this._validator = new TwitchAuthenticationValidator();
    }

    [Fact]
    public void ValidAuthenticationShouldPass()
    {
        TwitchAuthentication auth = new()
        {
            Api = new() { ClientId = "client-id", ClientSecret = "client-secret" },
            Chat = new() { UserName = "bot-user", OAuthToken = "oauth-token" },
        };

        ValidationResult result = this._validator.Validate(auth);

        Assert.True(condition: result.IsValid, userMessage: "Valid authentication should pass validation");
    }

    [Fact]
    public void InvalidApiShouldFail()
    {
        TwitchAuthentication auth = new()
        {
            Api = new() { ClientId = string.Empty, ClientSecret = "client-secret" },
            Chat = new() { UserName = "bot-user", OAuthToken = "oauth-token" },
        };

        ValidationResult result = this._validator.Validate(auth);

        Assert.False(condition: result.IsValid, userMessage: "Invalid Api should fail validation");
    }

    [Fact]
    public void InvalidChatShouldFail()
    {
        TwitchAuthentication auth = new()
        {
            Api = new() { ClientId = "client-id", ClientSecret = "client-secret" },
            Chat = new() { UserName = string.Empty, OAuthToken = "oauth-token" },
        };

        ValidationResult result = this._validator.Validate(auth);

        Assert.False(condition: result.IsValid, userMessage: "Invalid Chat should fail validation");
    }
}
