using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Database.Interfaces.Builders;

/// <summary>
///     Converts a collection of source entities into a collection of destination entities.
/// </summary>
/// <typeparam name="TSourceObject">The type of the source object.</typeparam>
/// <typeparam name="TDestinationObject">The type of the destination object.</typeparam>
public interface IObjectCollectionBuilder<in TSourceObject, out TDestinationObject> : IObjectBuilder<TSourceObject, TDestinationObject>
    where TSourceObject : class where TDestinationObject : class
{
    /// <summary>
    ///     Converts a collection of source entities into a collection of destination entities.
    /// </summary>
    /// <param name="entities">The source entities collection.</param>
    /// <returns>Collection of converted objects.</returns>
    IReadOnlyList<TDestinationObject> Build(IEnumerable<TSourceObject?> entities);
}