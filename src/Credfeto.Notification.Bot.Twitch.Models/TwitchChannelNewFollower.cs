using System;
using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{Streamer}: {Viewer} StreamOnline: {StreamOnline} IsStreamer: {IsStreamer} Created: {AccountCreated} FollowCount: {FollowCount}")]
public sealed record TwitchChannelNewFollower(in Streamer Streamer, in Viewer Viewer, bool StreamOnline, bool IsStreamer, in DateTimeOffset AccountCreated, int FollowCount) : INotification;