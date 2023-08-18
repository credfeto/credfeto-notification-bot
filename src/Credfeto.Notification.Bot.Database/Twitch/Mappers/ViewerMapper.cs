using System.Data;
using System.Data.Common;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.Mappers;

internal sealed class ViewerMapper : IMapper<Viewer>
{
    public static Viewer MapFromDb(object value)
    {
        return Viewer.FromString(value as string ?? throw new DataException("Unknown Viewer"));
    }

    public static void MapToDb(Viewer value, DbParameter parameter)
    {
        parameter.Value = value.Value;
    }
}