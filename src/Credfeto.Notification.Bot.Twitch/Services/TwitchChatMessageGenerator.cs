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
                                                                          "Thanks @{0} for the {1} bits!",
                                                                          "@{0}, Thanks for the bits!",
                                                                          "@{0}, Thanks for bits!",
                                                                          "@{0}, Thanks for the bits! You're awesome!",
                                                                          "@{0}, Thanks for the {1} bits! You're awesome!",
                                                                          "@{0}, Thanks for giving {1} bits!",
                                                                          "Thank you for the bits @{0}! VirtualHug",
                                                                          "Thank you for the {1} bits @{0}! VirtualHug",
                                                                          "Thanks for the bits @{0}!",
                                                                          "Thanks for the {1} bits @{0}!",
                                                                          "Thank you for cheering the bits @{0}!",
                                                                          "Thank you for cheering {1} bits @{0}"
                                                                      };

    private static readonly IReadOnlyList<string> ThankNewPrimeSubMessages = new[]
                                                                             {
                                                                                 "Thanks @{0} for subscribing!",
                                                                                 "Thanks @{0} for subscribing and using your prime sub here!"
                                                                             };

    private static readonly IReadOnlyList<string> ThankNewPaidSubMessages = new[]
                                                                            {
                                                                                "Thanks @{0} for subscribing!"
                                                                            };

    private static readonly IReadOnlyList<string> ThankPrimeReSubMessages = new[]
                                                                            {
                                                                                "Thanks @{0} for resubscribing!"
                                                                            };

    private static readonly IReadOnlyList<string> ThankPaidReSubMessages = new[]
                                                                           {
                                                                               "Thanks @{0} for resubscribing!"
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
        string formatString = this.GetMessage(ThankNewPrimeSubMessages);

        return string.Format(format: formatString, arg0: user.Value);
    }

    public string ThanksForPrimeReSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankPrimeReSubMessages);

        return string.Format(format: formatString, arg0: user.Value);
    }

    public string ThanksForPaidReSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankPaidReSubMessages);

        return string.Format(format: formatString, arg0: user.Value);
    }

    public string ThanksForNewPaidSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankNewPaidSubMessages);

        return string.Format(format: formatString, arg0: user.Value);
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