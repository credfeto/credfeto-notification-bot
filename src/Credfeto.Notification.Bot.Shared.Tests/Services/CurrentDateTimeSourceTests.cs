using System;
using System.Diagnostics.CodeAnalysis;
using Credfeto.Notification.Bot.Shared.Services;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Shared.Tests.Services;

public sealed class CurrentDateTimeSourceTests : TestBase
{
    private readonly ICurrentTimeSource _currentDateTimeSource;

    public CurrentDateTimeSourceTests()
    {
        this._currentDateTimeSource = new CurrentTimeSource();
    }

    [Fact]
    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0002: Call Date Time abstraction", Justification = "This is implementing the referenced mechanism")]
    public void CurrentDateTimeSource_GetUtcNow_ReturnsCurrentDateTime()
    {
        // Arrange
        DateTime expected = DateTime.UtcNow;
        ICurrentTimeSource currentDateTimeSource = this._currentDateTimeSource;

        // Act
        DateTime result = currentDateTimeSource.UtcNow();

        // Assert
        Assert.Equal(expected: expected, actual: result);
    }
}