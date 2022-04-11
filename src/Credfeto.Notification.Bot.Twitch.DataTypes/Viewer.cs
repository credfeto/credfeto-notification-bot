using System.Diagnostics.CodeAnalysis;
using StronglyTypedIds;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.String, converters: StronglyTypedIdConverter.None, implementations: StronglyTypedIdImplementations.IEquatable)]
[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Uses source generation")]
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