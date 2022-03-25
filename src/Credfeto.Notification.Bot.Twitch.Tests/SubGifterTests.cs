using System;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests;

public sealed class SubGifterTests : TestBase
{
    private const string INITIAL_GIFTER = "me";
    private readonly Context _context;
    private readonly ICurrentTimeSource _currentDateTimeSource;
    private readonly ILogger _logger;

    public SubGifterTests()
    {
        this._currentDateTimeSource = GetSubstitute<ICurrentTimeSource>();
        this._logger = this.GetTypedLogger<SubGifter>();

        this._context = new();

        this._currentDateTimeSource.UtcNow()
            .Returns(_ => this._context.Now());
    }

    [Theory]
    [InlineData(INITIAL_GIFTER, 100, true)]
    [InlineData(INITIAL_GIFTER, 4999, true)]
    [InlineData(INITIAL_GIFTER, 5000, false)]
    [InlineData(INITIAL_GIFTER, 5001, false)]
    [InlineData("other", 100, false)]
    [InlineData("other", 4999, false)]
    [InlineData("other", 5000, false)]
    [InlineData("other", 5001, false)]
    public void GifterCheck(string gifter, int advanceMs, bool shouldBeSameGifter)
    {
        SubGifter subGifter = new(giftedBy: INITIAL_GIFTER, currentTimeSource: this._currentDateTimeSource, logger: this._logger);

        this._context.Advance(TimeSpan.FromMilliseconds(advanceMs));

        bool sameUser = subGifter.Update(gifter);
        Assert.Equal(expected: shouldBeSameGifter, actual: sameUser);
    }

    private sealed class Context
    {
        private bool _advanced;
        private DateTime _when;

        public Context()
        {
            this._when = new(year: 2022, month: 1, day: 1, hour: 11, minute: 21, second: 1, millisecond: 15);
            this._advanced = true;
        }

        public DateTime Now()
        {
            Assert.True(condition: this._advanced, userMessage: "Time Should have advanced");
            this._advanced = false;

            return this._when;
        }

        public void Advance(in TimeSpan amount)
        {
            Assert.False(condition: this._advanced, userMessage: "Time Should not have advanced");
            this._when = this._when.Add(amount);
            this._advanced = true;
        }
    }
}