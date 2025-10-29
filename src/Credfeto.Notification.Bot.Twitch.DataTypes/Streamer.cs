using System.Diagnostics.CodeAnalysis;
using StronglyTypedIds;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[StronglyTypedId(Template.String)]

public readonly partial struct Streamer
{
    public static Streamer FromString(string username)
    {
        return new(username.ToLowerInvariant());
    }

    public Viewer ToViewer()
    {
        return new(this.Value);
    }
}
