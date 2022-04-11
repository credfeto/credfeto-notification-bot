using System;
using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

/// <summary>
///     A Twitch user.
/// </summary>
[DebuggerDisplay("{UserName}: Id: {Id} Streamer: {IsStreamer} Created: {DateCreated}")]
public sealed class TwitchUser
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="id">Unique Twitch User Id.</param>
    /// <param name="userName">The username.</param>
    /// <param name="isStreamer">True, if the user is a streamer; otherwise, null.</param>
    /// <param name="dateCreated">The date the user was created.</param>
    public TwitchUser(string id, in Viewer userName, bool isStreamer, in DateTime dateCreated)
    {
        this.Id = id;
        this.UserName = userName;
        this.IsStreamer = isStreamer;
        this.DateCreated = dateCreated;
    }

    /// <summary>
    ///     The Unique Twitch User Id.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     The username.
    /// </summary>
    public Viewer UserName { get; }

    /// <summary>
    ///     True, if the user is a streamer; otherwise, null.
    /// </summary>
    public bool IsStreamer { get; }

    /// <summary>
    ///     The date the user was created.
    /// </summary>
    public DateTime DateCreated { get; }
}