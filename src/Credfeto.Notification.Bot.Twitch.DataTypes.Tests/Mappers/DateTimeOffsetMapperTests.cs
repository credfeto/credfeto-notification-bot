using System;
using System.Data;
using System.Data.Common;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests.Mappers;

public sealed class DateTimeOffsetMapperTests : TestBase
{
    private static readonly DateTime TestDateTime = new(
        year: 2020,
        month: 6,
        day: 15,
        hour: 12,
        minute: 0,
        second: 0,
        kind: DateTimeKind.Utc
    );

    [Fact]
    public void MapFromDb_ConvertsDateTimeToDateTimeOffset()
    {
        DateTimeOffset result = DateTimeOffsetMapper.MapFromDb(TestDateTime);

        Assert.Equal(expected: new DateTimeOffset(TestDateTime), actual: result);
    }

    [Fact]
    public void MapToDb_SetsParameterValue()
    {
        DateTimeOffset value = new(TestDateTime);
        DbParameter parameter = GetSubstitute<DbParameter>();

        DateTimeOffsetMapper.MapToDb(value, parameter);

        DateTimeOffset actualValue = Assert.IsType<DateTimeOffset>(parameter.Value);
        Assert.Equal(expected: value, actual: actualValue);
    }

    [Fact]
    public void MapToDb_SetsParameterDbType()
    {
        DateTimeOffset value = new(TestDateTime);
        DbParameter parameter = GetSubstitute<DbParameter>();

        DateTimeOffsetMapper.MapToDb(value, parameter);

        Assert.Equal(expected: DbType.DateTimeOffset, actual: parameter.DbType);
    }
}
