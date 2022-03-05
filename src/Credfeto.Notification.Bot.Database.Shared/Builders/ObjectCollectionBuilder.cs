using System;
using System.Collections.Generic;
using System.Linq;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Shared.Extensions;

namespace Credfeto.Notification.Bot.Database.Shared.Builders;

/// <summary>
///     Converts a collection of source entities into a collection of destination entities.
/// </summary>
/// <typeparam name="TSourceObject">The type of the source object.</typeparam>
/// <typeparam name="TDestinationObject">The type of the destination object.</typeparam>
public sealed class ObjectCollectionBuilder<TSourceObject, TDestinationObject> : IObjectCollectionBuilder<TSourceObject, TDestinationObject>
    where TSourceObject : class where TDestinationObject : class
{
    private readonly IObjectBuilder<TSourceObject, TDestinationObject> _entityBuilder;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="entityBuilder">The entity builder.</param>
    public ObjectCollectionBuilder(IObjectBuilder<TSourceObject, TDestinationObject> entityBuilder)
    {
        this._entityBuilder = entityBuilder ?? throw new ArgumentNullException(nameof(entityBuilder));
    }

    /// <inheritdoc />
    public TDestinationObject? Build(TSourceObject? source)
    {
        return this._entityBuilder.Build(source);
    }

    /// <inheritdoc />
    public IReadOnlyList<TDestinationObject> Build(IEnumerable<TSourceObject?> entities)
    {
        return entities.Select(this.Build)
                       .RemoveNulls()
                       .ToArray();
    }
}