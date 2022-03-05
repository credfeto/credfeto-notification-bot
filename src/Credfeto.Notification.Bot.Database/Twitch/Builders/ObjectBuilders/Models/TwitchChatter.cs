using System;
using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Channel}: {ChatUser} {FirstMessage} Stream Started: {StreamStartDate}")]
public sealed class TwitchChatter
{
    public TwitchChatter(string channel, in DateTime streamStartDate, string chatUser, in DateTime firstMessage)
    {
        this.Channel = channel;
        this.StreamStartDate = streamStartDate;
        this.ChatUser = chatUser;
        this.FirstMessage = firstMessage;
    }

    public string Channel { get; }

    public DateTime StreamStartDate { get; }

    public string ChatUser { get; }

    public DateTime FirstMessage { get; }
}