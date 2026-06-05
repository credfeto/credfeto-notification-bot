using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Configuration.Validators;
using FluentValidation.Results;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Configuration.Validators;

public sealed class TwitchAuthenticationValidatorApiTests : TestBase
{
    private readonly TwitchAuthenticationValidatorApi _validator;

    public TwitchAuthenticationValidatorApiTests()
    {
        this._validator = new TwitchAuthenticationValidatorApi();
    }

    [Fact]
    public void ValidCredentialsShouldPass()
    {
        TwitchAuthenticationApi api = new() { ClientId = "client-id", ClientSecret = "client-secret" };

        ValidationResult result = this._validator.Validate(api);

        Assert.True(condition: result.IsValid, userMessage: "Valid credentials should pass validation");
    }

    [Fact]
    public void EmptyClientIdShouldFail()
    {
        TwitchAuthenticationApi api = new() { ClientId = string.Empty, ClientSecret = "client-secret" };

        ValidationResult result = this._validator.Validate(api);

        Assert.False(condition: result.IsValid, userMessage: "Empty ClientId should fail validation");
    }

    [Fact]
    public void EmptyClientSecretShouldFail()
    {
        TwitchAuthenticationApi api = new() { ClientId = "client-id", ClientSecret = string.Empty };

        ValidationResult result = this._validator.Validate(api);

        Assert.False(condition: result.IsValid, userMessage: "Empty ClientSecret should fail validation");
    }
}
