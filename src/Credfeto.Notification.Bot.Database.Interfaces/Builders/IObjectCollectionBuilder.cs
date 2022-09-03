using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Database.Interfaces.Builders;

public interface IObjectCollectionBuilder<in TSourceObject, out TDestinationObject> : IObjectBuilder<TSourceObject, TDestinationObject>
    where TSourceObject : class where TDestinationObject : class
{
    IReadOnlyList<TDestinationObject> Build(IEnumerable<TSourceObject> entities);
}