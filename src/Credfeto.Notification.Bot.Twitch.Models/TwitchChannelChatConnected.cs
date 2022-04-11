using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchChannelChatConnected : INotification
{
    public TwitchChannelChatConnected(in Streamer streamer)
    {
        this.Streamer = streamer;
    }

    public Streamer Streamer { get; }
}