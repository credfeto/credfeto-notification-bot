using System.Data;
using System.Data.Common;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests.Mappers;

public sealed class StreamerMapperTests : TestBase
{
    [Fact]
    public void MapFromDb_ReturnsStreamerFromString()
    {
        Streamer result = StreamerMapper.MapFromDb("TestStreamer");

        Assert.Equal(expected: "teststreamer", actual: result.Value);
    }

    [Fact]
    public void MapFromDb_ThrowsDataExceptionForNonString()
    {
        Assert.Throws<DataException>(() => StreamerMapper.MapFromDb(42));
    }

    [Fact]
    public void MapToDb_SetsParameterValue()
    {
        Streamer streamer = Streamer.FromString("teststreamer");
        DbParameter parameter = GetSubstitute<DbParameter>();

        StreamerMapper.MapToDb(streamer, parameter);

        Assert.Equal(expected: "teststreamer", actual: (string?)parameter.Value);
    }
}
