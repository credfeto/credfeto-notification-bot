using System;
using System.Diagnostics.CodeAnalysis;
using Credfeto.Notification.Bot.Shared;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class SubGifter
{
    private static readonly TimeSpan DeDupTime = TimeSpan.FromSeconds(30);
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ILogger _logger;
    private string _giftedBy;
    private DateTime _whenGifted;

    public SubGifter(string giftedBy,
                     ICurrentTimeSource currentTimeSource,
                     [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024:ILogger should be typed", Justification = "Not created by DI")] ILogger logger)
    {
        this._giftedBy = giftedBy;

        this._currentTimeSource = currentTimeSource;
        this._logger = logger;
        this._whenGifted = this._currentTimeSource.UtcNow();
    }

    public bool Update(string giftedBy)
    {
        DateTime now = this._currentTimeSource.UtcNow();
        TimeSpan duration = now - this._whenGifted;

        this._logger.LogInformation($"Last Gift {duration.TotalMilliseconds}ms ago (DeDup {DeDupTime.TotalMilliseconds}ms) by {this._giftedBy} at {now} ");

        if (StringComparer.InvariantCultureIgnoreCase.Equals(x: this._giftedBy, y: giftedBy))
        {
            this._whenGifted = now;
            this._logger.LogInformation($"Last Gift Time by {this._giftedBy} set to {now}");

            return duration < DeDupTime;
        }

        this._logger.LogInformation($"New Gifter :{giftedBy} Last Gift Time set to {now}");

        this._whenGifted = now;
        this._giftedBy = giftedBy;

        return false;
    }
}