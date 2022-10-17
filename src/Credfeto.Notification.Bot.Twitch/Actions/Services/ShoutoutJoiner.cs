using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ShoutoutJoiner : MessageSenderBase, IShoutoutJoiner
{
    private readonly ILogger<ShoutoutJoiner> _logger;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public ShoutoutJoiner(IOptions<TwitchBotOptions> options, ITwitchChannelManager twitchChannelManager, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<ShoutoutJoiner> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public async Task<bool> IssueShoutoutAsync(Streamer streamer, TwitchUser visitingStreamer, bool isRegular, CancellationToken cancellationToken)
    {
        try
        {
            ITwitchChannelState channelState = this._twitchChannelManager.GetStreamer(streamer);

            if (!channelState.Settings.ShoutOutsEnabled)
            {
                return false;
            }

            TwitchFriendChannel? twitchFriendChannel = this.FindTwitchFriendChannel(streamer: streamer, visitingStreamer: visitingStreamer);

            if (twitchFriendChannel != null)
            {
                return await this.IssueFriendChannelShoutoutAsync(streamer: streamer,
                                                                  visitingStreamer: visitingStreamer,
                                                                  twitchFriendChannel: twitchFriendChannel,
                                                                  cancellationToken: cancellationToken);
            }

            if (isRegular)
            {
                await this.SendStandardShoutoutAsync(streamer: streamer, visitingStreamer: visitingStreamer, code: "REGULAR", cancellationToken: cancellationToken);
                await this.SendBuiltInShoutoutAsync(streamer: streamer, visitingStreamer: visitingStreamer, code: "REGULAR", cancellationToken: cancellationToken);

                return true;
            }

            return this.NoShoutout(streamer: streamer, visitingStreamer: visitingStreamer);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{streamer}: Check Shoutout: Failed to check {exception.Message}");

            return false;
        }
    }

    private bool NoShoutout(in Streamer streamer, TwitchUser visitingStreamer)
    {
        this.LogShoutout(streamer: streamer, visitingStreamer: visitingStreamer, code: "NEW");

        return false;
    }

    private TwitchFriendChannel? FindTwitchFriendChannel(Streamer streamer, TwitchUser visitingStreamer)
    {
        TwitchModChannel? channel = Array.Find(array: this._options.Channels, match: c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.ChannelName, y: streamer.Value));

        return channel?.ShoutOuts.FriendChannels?.FirstOrDefault(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.Channel, y: visitingStreamer.UserName.Value));
    }

    private async Task<bool> IssueFriendChannelShoutoutAsync(Streamer streamer, TwitchUser visitingStreamer, TwitchFriendChannel twitchFriendChannel, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(twitchFriendChannel.Message))
        {
            await this.SendStandardShoutoutAsync(streamer: streamer, visitingStreamer: visitingStreamer, code: "FRIEND", cancellationToken: cancellationToken);
            await this.SendBuiltInShoutoutAsync(streamer: streamer, visitingStreamer: visitingStreamer, code: "FRIEND", cancellationToken: cancellationToken);
        }
        else
        {
            await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.SLOW, message: twitchFriendChannel.Message, cancellationToken: cancellationToken);
            await this.SendBuiltInShoutoutAsync(streamer: streamer, visitingStreamer: visitingStreamer, code: "FRIEND_MSG", cancellationToken: cancellationToken);
            this.LogShoutout(streamer: streamer, visitingStreamer: visitingStreamer, code: "FRIEND_MSG");
        }

        return true;
    }

    private async Task SendStandardShoutoutAsync(Streamer streamer, TwitchUser visitingStreamer, string code, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.SLOW, $"!so @{visitingStreamer.UserName}", cancellationToken: cancellationToken);
        this.LogShoutout(streamer: streamer, visitingStreamer: visitingStreamer, code: code);
    }

    private async Task SendBuiltInShoutoutAsync(Streamer streamer, TwitchUser visitingStreamer, string code, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.SLOW, $"/shoutout @{visitingStreamer.UserName}", cancellationToken: cancellationToken);
        this.LogShoutout(streamer: streamer, visitingStreamer: visitingStreamer, code: code);
    }

    private void LogShoutout(in Streamer streamer, TwitchUser visitingStreamer, string code)
    {
        this._logger.LogWarning($"{streamer}: Check out https://www.twitch.tv/{visitingStreamer.UserName}  [{code}]");
    }
}