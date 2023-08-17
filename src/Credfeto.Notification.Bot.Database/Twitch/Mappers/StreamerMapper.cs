using System.Data;
using System.Data.Common;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.Mappers;

internal sealed class StreamerMapper : IMapper<Streamer>
{
    public static Streamer MapFromDb(object value)
    {
        return Streamer.FromString(value as string ?? throw new DataException("Unknown Streamer"));
    }

    public static void MapToDb(Streamer value, DbParameter parameter)
    {
        parameter.Value = value.Value;
    }
}