using System.Diagnostics.CodeAnalysis;
using StronglyTypedIds;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[StronglyTypedId(Template.String)]
[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Uses source generation")]
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
