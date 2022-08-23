using System;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchMessageTriggerDebounceFilterTests : TestBase
{
    private static readonly DateTime Initial = new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);

    private static readonly TwitchMessageMatch MessageMatch = new(streamer: MockReferenceData.Streamer,
                                                                  chatter: MockReferenceData.Viewer,
                                                                  matchType: TwitchMessageMatchType.EXACT,
                                                                  message: "message");

    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;

    public TwitchMessageTriggerDebounceFilterTests()
    {
        this._currentTimeSource = GetSubstitute<ICurrentTimeSource>();
        this._twitchMessageTriggerDebounceFilter =
            new TwitchMessageTriggerDebounceFilter(currentTimeSource: this._currentTimeSource, this.GetTypedLogger<TwitchMessageTriggerDebounceFilter>());
    }

    private void MockCurrentTime(bool shortBreak)
    {
        DateTime currentTime = Initial;

        this._currentTimeSource.UtcNow()
            .Returns(_ =>
                     {
                         DateTime now = currentTime;
                         currentTime = currentTime.AddSeconds(shortBreak
                                                                  ? 1
                                                                  : 90);

                         return now;
                     });
    }

    [Fact]
    public void FirstMessageAlwaysSends()
    {
        this.MockCurrentTime(true);

        bool shouldSend = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
        Assert.True(condition: shouldSend, userMessage: "First message should always send");

        this.ReceivedUtcNow(1);
    }

    private void ReceivedUtcNow(int receiveCount)
    {
        this._currentTimeSource.Received(receiveCount)
            .UtcNow();
    }

    [Fact]
    public void MessageShortlyAfterPreviousShouldNotSend()
    {
        const int attempts = 100;

        this.MockCurrentTime(true);

        bool shouldSendFirst = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
        Assert.True(condition: shouldSendFirst, userMessage: "First message should always send");

        for (int attempt = 2; attempt <= attempts; attempt++)
        {
            bool shouldSendSecond = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
            Assert.False(condition: shouldSendSecond, $"Message should not send after {attempt} short break attempts");
        }

        this.ReceivedUtcNow(attempts);
    }

    [Fact]
    public void MessageAfterLongBreakShouldAlwaysSend()
    {
        const int attempts = 100;

        this.MockCurrentTime(false);

        bool shouldSendFirst = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
        Assert.True(condition: shouldSendFirst, userMessage: "First message should always send");

        for (int attempt = 2; attempt <= attempts; attempt++)
        {
            bool shouldSendSecond = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
            Assert.True(condition: shouldSendSecond, userMessage: "Second message should send");
        }

        this.ReceivedUtcNow(attempts);
    }

    [Fact]
    public void ThirdMessageWithBreakFirstSentSecondNotShouldSend()
    {
        this._currentTimeSource.UtcNow()
            .Returns(returnThis: Initial, Initial.AddSeconds(1), Initial.AddSeconds(91));

        bool shouldSendFirst = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
        Assert.True(condition: shouldSendFirst, userMessage: "First message should always send");

        bool shouldSendSecond = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
        Assert.False(condition: shouldSendSecond, userMessage: "Second message should not send");

        bool shouldSendThird = this._twitchMessageTriggerDebounceFilter.CanSend(MessageMatch);
        Assert.True(condition: shouldSendThird, userMessage: "Third message should send");
    }
}