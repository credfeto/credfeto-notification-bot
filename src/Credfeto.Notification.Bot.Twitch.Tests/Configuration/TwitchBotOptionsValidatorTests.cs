using System.Collections.Generic;
using Credfeto.Notification.Bot.Twitch.Configuration;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Configuration;

public sealed class TwitchBotOptionsValidatorTests : TestBase
{
    private readonly TwitchBotOptionsValidator _validator = new();

    [Fact]
    public void ValidOptionsPass()
    {
        ValidateOptionsResult result = this.Validate(
            CreateOptions(api: CreateApi(), chat: CreateChat(), chatCommands: [])
        );

        Assert.True(condition: result.Succeeded, userMessage: result.FailureMessage);
    }

    [Fact]
    public void ValidOptionsWithChatCommandsPass()
    {
        ValidateOptionsResult result = this.Validate(
            CreateOptions(api: CreateApi(), chat: CreateChat(), chatCommands: [CreateChatCommand()])
        );

        Assert.True(condition: result.Succeeded, userMessage: result.FailureMessage);
    }

    [Theory]
    [InlineData("", "client-secret")]
    [InlineData("client-id", "")]
    public void InvalidApiAuthenticationFails(string clientId, string clientSecret)
    {
        TwitchAuthenticationApi api = new() { ClientId = clientId, ClientSecret = clientSecret };

        ValidateOptionsResult result = this.Validate(CreateOptions(api: api, chat: CreateChat(), chatCommands: []));

        Assert.True(condition: result.Failed, userMessage: "Api authentication should be rejected");
    }

    [Theory]
    [InlineData("", "oauth-token")]
    [InlineData("bot-user", "")]
    public void InvalidChatAuthenticationFails(string userName, string oAuthToken)
    {
        TwitchAuthenticationChat chat = new() { UserName = userName, OAuthToken = oAuthToken };

        ValidateOptionsResult result = this.Validate(CreateOptions(api: CreateApi(), chat: chat, chatCommands: []));

        Assert.True(condition: result.Failed, userMessage: "Chat authentication should be rejected");
    }

    [Theory]
    [InlineData("", "bot", "!play", "!play")]
    [InlineData("streamer", "", "!play", "!play")]
    [InlineData("streamer", "bot", "", "!play")]
    [InlineData("streamer", "bot", "!play", "")]
    public void InvalidChatCommandInListFails(string streamer, string bot, string match, string issue)
    {
        TwitchChatCommand invalid = new(streamer: streamer, bot: bot, match: match, issue: issue, matchType: "EXACT");

        ValidateOptionsResult result = this.Validate(
            CreateOptions(api: CreateApi(), chat: CreateChat(), chatCommands: [CreateChatCommand(), invalid])
        );

        Assert.True(condition: result.Failed, userMessage: "Chat command should be rejected");
    }

    private ValidateOptionsResult Validate(TwitchBotOptions options)
    {
        return this._validator.Validate(name: null, options: options);
    }

    private static TwitchBotOptions CreateOptions(
        TwitchAuthenticationApi api,
        TwitchAuthenticationChat chat,
        List<TwitchChatCommand> chatCommands
    )
    {
        return new()
        {
            Authentication = new() { Api = api, Chat = chat },
            ChatCommands = chatCommands,
        };
    }

    private static TwitchAuthenticationApi CreateApi()
    {
        return new() { ClientId = "client-id", ClientSecret = "client-secret" };
    }

    private static TwitchAuthenticationChat CreateChat()
    {
        return new() { UserName = "bot-user", OAuthToken = "oauth-token" };
    }

    private static TwitchChatCommand CreateChatCommand()
    {
        return new(streamer: "streamer", bot: "bot", match: "!play", issue: "!play", matchType: "EXACT");
    }
}
