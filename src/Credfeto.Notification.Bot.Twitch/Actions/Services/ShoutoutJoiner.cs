using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
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

        foreach (TwitchChannelShoutout channel in this._options.Shoutouts)
        {
            foreach (TwitchFriendChannel visitingStreamer in channel.FriendChannels)
            {
                this._logger.LogWarning($"{channel.Channel}: Has Friend for shoutouts - {visitingStreamer.Channel}");
            }
        }
    }

    public async Task<bool> IssueShoutoutAsync(string channel, TwitchUser visitingStreamer, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{channel}: Checking if need to shoutout {visitingStreamer.UserName}");
        TwitchChannelShoutout? soChannel = this._options.Shoutouts.Find(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.Channel, y: channel));

        if (soChannel == null)
        {
            this._logger.LogInformation($"{channel}: Shout-outs not enabled");

            return false;
        }

        TwitchFriendChannel? streamer = soChannel.FriendChannels.Find(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.Channel, y: visitingStreamer.UserName));

        if (streamer == null)
        {
            foreach (TwitchFriendChannel c in soChannel.FriendChannels)
            {
                this._logger.LogInformation($"{channel}: Found Friend: {c.Channel}");
            }

            // TODO: Check to see if the user has streamed recently using API
            // TODO: Can mod comments be read?
            this._logger.LogWarning($"{channel}: Check out https://www.twitch.tv/{visitingStreamer.UserName}");

            // TODO: Log in DB and id becomes a regular then shout them out

            return false;
        }

        this._logger.LogInformation($"{channel}: Check out https://www.twitch.tv/{visitingStreamer.UserName}");

        if (string.IsNullOrWhiteSpace(streamer.Message))
        {
            await this.SendMessageAsync(channel: channel, $"Check out https://www.twitch.tv/{visitingStreamer.UserName}", cancellationToken: cancellationToken);
        }
        else
        {
            await this.SendMessageAsync(channel: channel, message: streamer.Message, cancellationToken: cancellationToken);
        }

        return true;
    }
}