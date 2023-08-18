using System;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Credfeto.Database.Interfaces;

namespace Credfeto.Notification.Bot.Database.Twitch.Mappers;

internal sealed class DateTimeOffsetMapper : IMapper<DateTimeOffset>
{
    public static DateTimeOffset MapFromDb(object value)
    {
        return new(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
    }

    public static void MapToDb(DateTimeOffset value, DbParameter parameter)
    {
        parameter.Value = value;
        parameter.DbType = DbType.DateTimeOffset;
    }
}