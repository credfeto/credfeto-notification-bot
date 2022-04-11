using System;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Services;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.StreamState;

public sealed class SubDonorTrackerTests : TestBase
{
    private const string INITIAL_DONOR = "me";
    private readonly Context _context;
    private readonly ICurrentTimeSource _currentDateTimeSource;
    private readonly ILogger _logger;

    public SubDonorTrackerTests()
    {
        this._currentDateTimeSource = GetSubstitute<ICurrentTimeSource>();
        this._logger = this.GetTypedLogger<SubDonorTracker>();

        this._context = new();

        this._currentDateTimeSource.UtcNow()
            .Returns(_ => this._context.Now());
    }

    [Theory]
    [InlineData(INITIAL_DONOR, 100, true)]
    [InlineData(INITIAL_DONOR, 4999, true)]
    [InlineData(INITIAL_DONOR, 5000, false)]
    [InlineData(INITIAL_DONOR, 5001, false)]
    [InlineData("other", 100, false)]
    [InlineData("other", 4999, false)]
    [InlineData("other", 5000, false)]
    [InlineData("other", 5001, false)]
    public void DonorCheck(string donor, int advanceMs, bool shouldBeSameGifter)
    {
        SubDonorTracker subDonorTracker = new(currentTimeSource: this._currentDateTimeSource, logger: this._logger);

        bool firstUser = subDonorTracker.Update(Types.UserFromString(INITIAL_DONOR));
        Assert.False(condition: firstUser, userMessage: "First user should never be the 'same' user");

        this._context.Advance(TimeSpan.FromMilliseconds(advanceMs));

        bool sameUser = subDonorTracker.Update(Types.UserFromString(donor));
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