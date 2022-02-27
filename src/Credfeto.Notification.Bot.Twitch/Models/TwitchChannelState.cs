using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchChannelState
{
    private readonly string _channelName;
    private readonly ILogger _logger;
    private readonly TwitchBotOptions _options;
    private readonly IRaidWelcome _raidWelcome;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private ActiveStream? _stream;

    public TwitchChannelState(string channelName,
                              TwitchBotOptions options,
                              IRaidWelcome raidWelcome,
                              IShoutoutJoiner shoutoutJoiner,
                              [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024:ILogger should be typed", Justification = "Not created by DI")] ILogger logger)
    {
        this._channelName = channelName;
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        this._raidWelcome = raidWelcome ?? throw new ArgumentNullException(nameof(raidWelcome));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Online(string gameName, in DateTime startDate)
    {
        // TODO: Implement
        this._stream = new(gameName: gameName, startedAt: startDate);
    }

    public void Offline()
    {
        // TODO: Implement
        this._stream = null;
    }

    public void ClearChat()
    {
        // TODO: Implement
        this._stream?.AddIncident();
    }

    public async Task RaidedAsync(string raider, string viewerCount, CancellationToken cancellationToken)
    {
        if (this._stream?.AddRaider(raider: raider, viewerCount: viewerCount) == true)
        {
            if (this._options.RaidWelcomeEnabled(this._channelName))
            {
                await this._raidWelcome.IssueRaidWelcomeAsync(channel: this._channelName, raider: raider, cancellationToken: cancellationToken);
            }
        }
    }

    public bool ChatMessage(string user, string message, int bits)
    {
        if (bits != 0)
        {
            this._stream?.AddBitGifter(user: user, bits: bits);
        }

        // TODO: Implement
        return this._stream?.AddChatter(user) == true;
    }

    public void GiftedMultiple(string giftedBy, int count, string months)
    {
        this._stream?.GiftedSub(giftedBy: giftedBy, count: count);
    }

    public void GiftedSub(string giftedBy, string months)
    {
        this._stream?.GiftedSub(giftedBy: giftedBy, count: 1);
    }

    public void ContinuedSub(string user)
    {
        this._stream?.ContinuedSub(user);
    }

    public void PrimeToPaid(string user)
    {
        this._stream?.PrimeToPaid(user);
    }

    public void NewSubscriberPaid(string user)
    {
        this._stream?.NewSubscriberPaid(user);
    }

    public void NewSubscriberPrime(string user)
    {
        this._stream?.NewSubscriberPrime(user);
    }

    public void ResubscribePaid(string user, int months)
    {
        this._stream?.ResubscribePaid(user);
    }

    public void ResubscribePrime(string user, int months)
    {
        this._stream?.ResubscribePrime(user);
    }
}