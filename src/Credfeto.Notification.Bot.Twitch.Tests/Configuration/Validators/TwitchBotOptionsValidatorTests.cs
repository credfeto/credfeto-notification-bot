using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Configuration.Validators;
using FluentValidation.Results;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Configuration.Validators;

public sealed class TwitchBotOptionsValidatorTests : TestBase
{
    private static readonly TwitchAuthentication ValidAuthentication = new()
    {
        Api = new() { ClientId = "client-id", ClientSecret = "client-secret" },
        Chat = new() { UserName = "bot-user", OAuthToken = "oauth-token" },
    };

    private readonly TwitchBotOptionsValidator _validator;

    public TwitchBotOptionsValidatorTests()
    {
        this._validator = new TwitchBotOptionsValidator();
    }

    [Fact]
    public void ValidOptionsShouldPass()
    {
        TwitchBotOptions options = new() { Authentication = ValidAuthentication, ChatCommands = [] };

        ValidationResult result = this._validator.Validate(options);

        Assert.True(condition: result.IsValid, userMessage: "Valid options should pass validation");
    }

    [Fact]
    public void InvalidAuthenticationShouldFail()
    {
        TwitchBotOptions options = new()
        {
            Authentication = new()
            {
                Api = new() { ClientId = string.Empty, ClientSecret = "secret" },
                Chat = new() { UserName = "user", OAuthToken = "token" },
            },
            ChatCommands = [],
        };

        ValidationResult result = this._validator.Validate(options);

        Assert.False(condition: result.IsValid, userMessage: "Invalid Authentication should fail validation");
    }

    [Fact]
    public void ValidOptionsWithCommandsShouldPass()
    {
        TwitchBotOptions options = new()
        {
            Authentication = ValidAuthentication,
            ChatCommands = [new("streamer", "bot", "!play", "!play", "EXACT")],
        };

        ValidationResult result = this._validator.Validate(options);

        Assert.True(condition: result.IsValid, userMessage: "Valid options with commands should pass validation");
    }

    [Fact]
    public void InvalidCommandInListShouldFail()
    {
        TwitchBotOptions options = new()
        {
            Authentication = ValidAuthentication,
            ChatCommands =
            [
                new(streamer: string.Empty, bot: "bot", match: "!play", issue: "!play", matchType: "EXACT"),
            ],
        };

        ValidationResult result = this._validator.Validate(options);

        Assert.False(condition: result.IsValid, userMessage: "Invalid command in list should fail validation");
    }
}
