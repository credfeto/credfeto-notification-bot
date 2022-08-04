using System;
using System.Collections.Generic;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChatMessageGenerator : ITwitchChatMessageGenerator
{
    private static readonly IReadOnlyList<string> ThankBitsMessages = new[]
                                                                      {
                                                                          "Thanks @{0} for the bits!",
                                                                          "Thanks @{0} for the bits! You're awesome!",
                                                                          "Thanks @{0} for bits!",
                                                                          "Thanks @{0} for giving {1} bits!",
                                                                          "@{0}, Thanks for the bits!",
                                                                          "@{0}, Thanks for bits!",
                                                                          "@{0}, Thanks for the bits! You're awesome!",
                                                                          "@{0}, Thanks for giving {1} bits!"
                                                                      };

    private readonly IRandomNumberGenerator _randomNumberGenerator;

    public TwitchChatMessageGenerator(IRandomNumberGenerator randomNumberGenerator)
    {
        this._randomNumberGenerator = randomNumberGenerator ?? throw new ArgumentNullException(nameof(randomNumberGenerator));
    }

    public string ThanksForBits(in Viewer giftedBy, int bitsGiven)
    {
        string formatString = this.GetMessage(ThankBitsMessages);

        return string.Format(format: formatString, arg0: giftedBy.Value, arg1: bitsGiven);
    }

    public string ThanksForNewPrimeSub(in Viewer user)
    {
        return $"Thanks @{user} for subscribing";
    }

    public string ThanksForPrimeReSub(in Viewer user)
    {
        return $"Thanks @{user} for resubscribing";
    }

    public string ThanksForPaidReSub(in Viewer user)
    {
        return $"Thanks @{user} for resubscribing";
    }

    public string ThanksForNewPaidSub(in Viewer user)
    {
        return $"Thanks @{user} for subscribing";
    }

    public string ThanksForGiftingMultipleSubs(in Viewer giftedBy)
    {
        return $"Thanks @{giftedBy} for gifting subs";
    }

    public string ThanksForGiftingOneSub(in Viewer giftedBy)
    {
        return $"Thanks @{giftedBy} for gifting sub";
    }

    public string WelcomeMessage(in Viewer user)
    {
        return $"Hi @{user}";
    }

    private string GetMessage(IReadOnlyList<string> messages)
    {
        int messageIndex = this._randomNumberGenerator.Next(messages.Count);

        return messages[messageIndex];
    }
}