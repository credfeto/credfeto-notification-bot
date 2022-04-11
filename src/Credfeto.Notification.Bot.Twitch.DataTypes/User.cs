using StronglyTypedIds;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.String, converters: StronglyTypedIdConverter.None, implementations: StronglyTypedIdImplementations.IEquatable)]

// ReSharper disable once PartialTypeWithSinglePart
public readonly partial struct User
{
}