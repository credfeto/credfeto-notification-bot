using System.Data;
using System.Data.Common;
using Credfeto.Database.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

public sealed class ViewerMapper : IMapper<Viewer>
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
