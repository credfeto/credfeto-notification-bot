namespace Credfeto.Notification.Bot.Database.Interfaces.Builders;

/// <summary>
///     Converts the <typeparamref name="TSourceObject" /> into a <typeparamref name="TDestinationObject" />.
/// </summary>
/// <typeparam name="TSourceObject">The type of the source object.</typeparam>
/// <typeparam name="TDestinationObject">The type of the destination object.</typeparam>
public interface IObjectBuilder<in TSourceObject, out TDestinationObject>
    where TSourceObject : class where TDestinationObject : class
{
    /// <summary>
    ///     Converts the <typeparamref name="TSourceObject" /> into a <typeparamref name="TDestinationObject" />.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <returns>The destination object.</returns>
    TDestinationObject? Build(TSourceObject? source);
}