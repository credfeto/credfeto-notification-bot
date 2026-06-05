using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Configuration.Validators;
using FluentValidation.Results;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Configuration.Validators;

public sealed class TwitchAuthenticationValidatorChatTests : TestBase
{
    private readonly TwitchAuthenticationValidatorChat _validator;

    public TwitchAuthenticationValidatorChatTests()
    {
        this._validator = new TwitchAuthenticationValidatorChat();
    }

    [Fact]
    public void ValidCredentialsShouldPass()
    {
        TwitchAuthenticationChat chat = new() { UserName = "bot-user", OAuthToken = "oauth-token" };

        ValidationResult result = this._validator.Validate(chat);

        Assert.True(condition: result.IsValid, userMessage: "Valid credentials should pass validation");
    }

    [Fact]
    public void EmptyUserNameShouldFail()
    {
        TwitchAuthenticationChat chat = new() { UserName = string.Empty, OAuthToken = "oauth-token" };

        ValidationResult result = this._validator.Validate(chat);

        Assert.False(condition: result.IsValid, userMessage: "Empty UserName should fail validation");
    }

    [Fact]
    public void EmptyOAuthTokenShouldFail()
    {
        TwitchAuthenticationChat chat = new() { UserName = "bot-user", OAuthToken = string.Empty };

        ValidationResult result = this._validator.Validate(chat);

        Assert.False(condition: result.IsValid, userMessage: "Empty OAuthToken should fail validation");
    }
}
