using System.Diagnostics.CodeAnalysis;
using StronglyTypedIds;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[StronglyTypedId(Template.String)]

public readonly partial struct Viewer
{
    public static Viewer FromString(string user)
    {
        return new(user.ToLowerInvariant());
    }

    public Streamer ToStreamer()
    {
        return new(this.Value);
    }
}
