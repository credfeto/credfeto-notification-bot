using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Configuration.Validators;
using FluentValidation.Results;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Configuration.Validators;

public sealed class TwitchChatCommandValidatorTests : TestBase
{
    private readonly TwitchChatCommandValidator _validator;

    public TwitchChatCommandValidatorTests()
    {
        this._validator = new TwitchChatCommandValidator();
    }

    [Fact]
    public void ValidCommandShouldPass()
    {
        TwitchChatCommand command = new("streamer", "bot", "!play", "!play", "EXACT");

        ValidationResult result = this._validator.Validate(command);

        Assert.True(condition: result.IsValid, userMessage: "Valid command should pass validation");
    }

    [Fact]
    public void EmptyStreamerShouldFail()
    {
        TwitchChatCommand command = new(
            streamer: string.Empty,
            bot: "bot",
            match: "!play",
            issue: "!play",
            matchType: "EXACT"
        );

        ValidationResult result = this._validator.Validate(command);

        Assert.False(condition: result.IsValid, userMessage: "Empty streamer should fail validation");
    }

    [Fact]
    public void EmptyBotShouldFail()
    {
        TwitchChatCommand command = new(
            streamer: "streamer",
            bot: string.Empty,
            match: "!play",
            issue: "!play",
            matchType: "EXACT"
        );

        ValidationResult result = this._validator.Validate(command);

        Assert.False(condition: result.IsValid, userMessage: "Empty bot should fail validation");
    }

    [Fact]
    public void EmptyMatchShouldFail()
    {
        TwitchChatCommand command = new(
            streamer: "streamer",
            bot: "bot",
            match: string.Empty,
            issue: "!play",
            matchType: "EXACT"
        );

        ValidationResult result = this._validator.Validate(command);

        Assert.False(condition: result.IsValid, userMessage: "Empty match should fail validation");
    }

    [Fact]
    public void EmptyIssueShouldFail()
    {
        TwitchChatCommand command = new(
            streamer: "streamer",
            bot: "bot",
            match: "!play",
            issue: string.Empty,
            matchType: "EXACT"
        );

        ValidationResult result = this._validator.Validate(command);

        Assert.False(condition: result.IsValid, userMessage: "Empty issue should fail validation");
    }
}
