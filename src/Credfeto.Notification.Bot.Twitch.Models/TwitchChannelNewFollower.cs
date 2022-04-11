using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchChannelNewFollower : INotification
{
    public TwitchChannelNewFollower(in Channel channel, in User user, bool streamOnline, bool isStreamer, in DateTime accountCreated, int followCount)
    {
        this.Channel = channel;
        this.User = user;
        this.StreamOnline = streamOnline;
        this.IsStreamer = isStreamer;
        this.AccountCreated = accountCreated;
        this.FollowCount = followCount;
    }

    public Channel Channel { get; }

    public User User { get; }

    public bool StreamOnline { get; }

    public bool IsStreamer { get; }

    public DateTime AccountCreated { get; }

    public int FollowCount { get; }
}