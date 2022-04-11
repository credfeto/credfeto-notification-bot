using System.Diagnostics.CodeAnalysis;
using StronglyTypedIds;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.String, converters: StronglyTypedIdConverter.None, implementations: StronglyTypedIdImplementations.IEquatable)]
[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Uses source generation")]
public readonly partial struct Streamer
{
    public static Streamer FromString(string username)
    {
        return new(username.ToLowerInvariant());
    }

    public Viewer ToUser()
    {
        return new(this.Value);
    }
}