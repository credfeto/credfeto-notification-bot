using System.Data;
using System.Data.Common;
using Credfeto.Database.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

public sealed class StreamerMapper : IMapper<Streamer>
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