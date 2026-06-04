using System.Data;
using System.Data.Common;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests.Mappers;

public sealed class ViewerMapperTests : TestBase
{
    [Fact]
    public void MapFromDb_ReturnsViewerFromString()
    {
        Viewer result = ViewerMapper.MapFromDb("TestViewer");

        Assert.Equal(expected: "testviewer", actual: result.Value);
    }

    [Fact]
    public void MapFromDb_ThrowsDataExceptionForNonString()
    {
        Assert.Throws<DataException>(() => ViewerMapper.MapFromDb(42));
    }

    [Fact]
    public void MapToDb_SetsParameterValue()
    {
        Viewer viewer = Viewer.FromString("testviewer");
        DbParameter parameter = GetSubstitute<DbParameter>();

        ViewerMapper.MapToDb(viewer, parameter);

        Assert.Equal(expected: "testviewer", actual: (string?)parameter.Value);
    }
}
