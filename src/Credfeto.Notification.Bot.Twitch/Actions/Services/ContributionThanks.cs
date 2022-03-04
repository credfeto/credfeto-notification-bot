using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ContributionThanks : MessageSenderBase, IContributionThanks
{
    private readonly ICurrentTimeSource _currentTimeSource;

    private readonly ConcurrentDictionary<string, SubGifter> _gifters;
    private readonly ILogger<ContributionThanks> _logger;

    public ContributionThanks(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ICurrentTimeSource currentTimeSource, ILogger<ContributionThanks> logger)
        : base(twitchChatMessageChannel)
    {
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._gifters = new(StringComparer.CurrentCultureIgnoreCase);
    }

    public async Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for the bits.", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for for the bits.");
    }

    public async Task ThankForPrimeSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Paid)");
    }

    public Task ThankForMultipleGiftSubsAsync(string channelName, string giftedBy, int count, CancellationToken cancellationToken)
    {
        //await this.SendMessageAsync(channel: channelName, $"Thanks @{giftedBy} for gifting subs.", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channelName}: Thanks @{giftedBy} for gifting subs.");

        return Task.CompletedTask;
    }

    public async Task ThankForGiftingSubAsync(string channelName, string giftedBy, CancellationToken cancellationToken)
    {
#if FALSE
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting subs.
 steveforward: jimminewtron666: hell yeah! stevef18Woah stevef18Hornsofrock stevef18Woah stevef18Hornsofrock stevef18Woah stevef18Hornsofrock stevef18Woah stevef18Hornsofrock stevef18Woah stevef18Hornsofrock stevef18Woah stevef18Hornsofrock philsmLIT philsmLIT philsmLIT philsmLIT philsmLIT philsmLIT
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
 steveforward: lou_vella: lfsHH lfsHH lfsHH lfsHH lfsHH lfsHH lfsHH lfsHH
 steveforward: Community Sub: Lou_Vella
 steveforward: Thanks Lou_Vella for gifting sub.
#endif
        if (this.WasLastGifter(channel: channelName, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{channelName}: Thanks @{giftedBy} for gifting sub (Same as last gifter).");

            return;
        }

        await this.SendMessageAsync(channel: channelName, $"Thanks @{giftedBy} for gifting sub.", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channelName}: Thanks @{giftedBy} for gifting sub.");
    }

    private bool WasLastGifter(string channel, string giftedBy)
    {
        SubGifter subGifter = this._gifters.GetOrAdd(key: channel, new SubGifter(giftedBy: giftedBy, currentTimeSource: this._currentTimeSource));

        return subGifter.Update(giftedBy);
    }

    private sealed class SubGifter
    {
        private readonly ICurrentTimeSource _currentTimeSource;
        private string _giftedBy;
        private DateTime _whenGifted;

        public SubGifter(string giftedBy, ICurrentTimeSource currentTimeSource)
        {
            this._giftedBy = giftedBy;

            this._currentTimeSource = currentTimeSource;
            this._whenGifted = this._currentTimeSource.UtcNow();
        }

        public bool Update(string giftedBy)
        {
            DateTime now = this._currentTimeSource.UtcNow();
            TimeSpan duration = now - this._whenGifted;

            if (duration > TimeSpan.FromSeconds(10) && StringComparer.InvariantCultureIgnoreCase.Equals(x: this._giftedBy, y: giftedBy))
            {
                this._whenGifted = now;

                return false;
            }

            this._whenGifted = now;
            this._giftedBy = giftedBy;

            return false;
        }
    }
}