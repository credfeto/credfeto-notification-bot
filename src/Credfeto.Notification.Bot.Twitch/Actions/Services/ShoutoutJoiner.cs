using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ShoutoutJoiner : MessageSenderBase, IShoutoutJoiner
{
    private readonly ILogger<ShoutoutJoiner> _logger;
    private readonly TwitchBotOptions _options;

    public ShoutoutJoiner(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<ShoutoutJoiner> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public async Task<bool> IssueShoutoutAsync(string channel, TwitchUser visitingStreamer, bool isRegular, CancellationToken cancellationToken)
    {
        try
        {
            TwitchModChannel? modChannel = this._options.GetModChannel(channel);

            if (modChannel?.ShoutOuts.Enabled != true)
            {
                return false;
            }

            TwitchFriendChannel? streamer = modChannel.ShoutOuts.FriendChannels.Find(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.Channel, y: visitingStreamer.UserName));

            if (streamer == null)
            {
                if (isRegular)
                {
                    await this.SendStandardShoutoutAsync(channel: channel, visitingStreamer: visitingStreamer, code: "REGULAR", cancellationToken: cancellationToken);

                    return true;
                }

                this.LogShoutout(channel: channel, visitingStreamer: visitingStreamer, code: "NEW");

                return false;
            }

            if (string.IsNullOrWhiteSpace(streamer.Message))
            {
                await this.SendStandardShoutoutAsync(channel: channel, visitingStreamer: visitingStreamer, code: "FRIEND", cancellationToken: cancellationToken);
            }
            else
            {
                await this.SendMessageAsync(channel: channel, message: streamer.Message, cancellationToken: cancellationToken);
                this.LogShoutout(channel: channel, visitingStreamer: visitingStreamer, code: "FRIEND_MSG");
            }

            return true;
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{channel}: Check Shoutout: Failed to check {exception.Message}");

            return false;
        }
    }

    private async Task SendStandardShoutoutAsync(string channel, TwitchUser visitingStreamer, string code, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"!so @{visitingStreamer.UserName}", cancellationToken: cancellationToken);
        this.LogShoutout(channel: channel, visitingStreamer: visitingStreamer, code: code);
    }

    private void LogShoutout(string channel, TwitchUser visitingStreamer, string code)
    {
        this._logger.LogWarning($"{channel}: Check out https://www.twitch.tv/{visitingStreamer.UserName}  [{code}]");
    }
}