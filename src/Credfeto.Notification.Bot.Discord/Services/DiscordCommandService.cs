using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Discord.Commands;
using Credfeto.Notification.Bot.Shared;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Services;

/// <summary>
///     Command handling service.
/// </summary>
public sealed class DiscordCommandService : DiscordLoggingBase, IRunOnStartup
{
    private const string COMMAND_CHANNEL_NAME = "bot-commands";

    private readonly CommandService _commandService;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly IServiceProvider _serviceProvider;

    public DiscordCommandService(DiscordSocketClient discordSocketClient, CommandService commandService, IServiceProvider serviceProvider, ILogger<DiscordCommandService> logger)
        : base(logger)
    {
        this._commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        this._discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
        this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        // Hook CommandExecuted to handle post-command-execution logic.
        this._commandService.CommandExecuted += CommandExecutedAsync;

        // Hook MessageReceived so we can process each message to see
        // if it qualifies as a command.
        this._discordSocketClient.MessageReceived += this.MessageReceivedAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this._commandService.AddModuleAsync(typeof(PingModule), services: this._serviceProvider);
        await this._commandService.AddModuleAsync(typeof(TwitchModule), services: this._serviceProvider);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        // Ignore system messages, or messages from other bots
        if (rawMessage is not SocketUserMessage message)
        {
            return;
        }

        // don't respond to messages in general, access to all other channels can be controlled with
        // permissions on discord
        if (!StringComparer.InvariantCultureIgnoreCase.Equals(x: message.Channel.Name, y: COMMAND_CHANNEL_NAME))
        {
            return;
        }

        if (message.Source != MessageSource.User)
        {
            return;
        }

        // This value holds the offset where the prefix ends
        int argPos = 0;

        // Perform prefix check. You may want to replace this with
        // (!message.HasCharPrefix('!', ref argPos))
        // for a more traditional command format like !help.
        if (message.HasCharPrefix(c: '!', argPos: ref argPos))
        {
            await this.DispatchCommandAsync(message: message, argPos: argPos);

            return;
        }

        if (!message.HasMentionPrefix(user: this._discordSocketClient.CurrentUser, argPos: ref argPos))
        {
            await this.DispatchCommandAsync(message: message, argPos: argPos);
        }
    }

    private Task DispatchCommandAsync(SocketUserMessage message, int argPos)
    {
        SocketCommandContext context = new(client: this._discordSocketClient, msg: message);

        // Perform the execution of the command. In this method,
        // the command service will perform precondition and parsing check
        // then execute the command if one is matched.
        return this._commandService.ExecuteAsync(context: context, argPos: argPos, services: this._serviceProvider);

        // Note that normally a result will be returned by this format, but here
        // we will handle the result in CommandExecutedAsync,
    }

    private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        // command is unspecified when there was a search failure (command not found); we don't care about these errors
        if (!command.IsSpecified)
        {
            return;
        }

        // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
        if (result.IsSuccess)
        {
            return;
        }

        // the command failed, let's notify the user that something happened.
        await context.Channel.SendMessageAsync($"error: {result.ErrorReason}");
    }
}