using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState.LoggingExtensions;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{Streamer}")]
public sealed class TwitchChannelState : ITwitchChannelState
{
    private readonly ILogger _logger;

    private ActiveStream? _stream;

    public TwitchChannelState(
        in Streamer streamerStreamer,
        [SuppressMessage(
            category: "FunFair.CodeAnalysis",
            checkId: "FFS0024:ILogger should be typed",
            Justification = "Not created by DI"
        )]
            ILogger logger
    )
    {
        this.Streamer = streamerStreamer;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Streamer Streamer { get; }

    public bool IsOnline => this._stream != null;

    public ValueTask OnlineAsync(string gameName, DateTimeOffset startDate, CancellationToken cancellationToken)
    {
        this._logger.ChannelGoingOnline(this.Streamer);
        ActiveStream stream = new(gameName: gameName, startedAt: startDate);
        this._stream = stream;

        return ValueTask.CompletedTask;
    }

    public void Offline()
    {
        this._logger.ChannelGoingOffline(this.Streamer);
        this._stream = null;
    }

    public bool Chatted
    {
        get => this._stream?.UserChatted == true;
        set
        {
            if (this._stream is not null)
            {
                SetHasChatted(stream: this._stream, value: value);
            }
        }
    }

    private static void SetHasChatted(ActiveStream stream, bool value)
    {
        stream.UserChatted = value;
    }
}
