using System;
using System.Collections.Generic;
using System.Globalization;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Random.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChatMessageGenerator : ITwitchChatMessageGenerator
{
    private static readonly IReadOnlyList<string> ThankBitsMessages =
    [
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
    ];

    private static readonly IReadOnlyList<string> ThankNewPrimeSubMessages =
    [
        "Thanks @{0} for subscribing!",
        "Thanks @{0} for subscribing and using your prime sub here!"
    ];

    private static readonly IReadOnlyList<string> ThankNewPaidSubMessages =
    [
        "Thanks @{0} for subscribing!"
    ];

    private static readonly IReadOnlyList<string> ThankPrimeReSubMessages =
    [
        "Thanks @{0} for resubscribing!"
    ];

    private static readonly IReadOnlyList<string> ThankPaidReSubMessages =
    [
        "Thanks @{0} for resubscribing!"
    ];

    private static readonly IReadOnlyList<string> ThankGiftMultipleSubsMessages =
    [
        "Thanks @{0} for gifting subs!"
    ];

    private static readonly IReadOnlyList<string> ThankGiftOneSubMessages =
    [
        "Thanks @{0} for gifting sub!"
    ];

    private static readonly IReadOnlyList<string> WelcomeMessages =
    [
        "Hi @{0}",
        "Hi, @{0}",
        "@{0}, Hi!",
        "Hey, @{0}!"
    ];

    private readonly IRandomNumberGenerator _randomNumberGenerator;

    public TwitchChatMessageGenerator(IRandomNumberGenerator randomNumberGenerator)
    {
        this._randomNumberGenerator = randomNumberGenerator ?? throw new ArgumentNullException(nameof(randomNumberGenerator));
    }

    public string ThanksForBits(in Viewer giftedBy, int bitsGiven)
    {
        string formatString = this.GetMessage(ThankBitsMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: giftedBy.Value, arg1: bitsGiven);
    }

    public string ThanksForNewPrimeSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankNewPrimeSubMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: user.Value);
    }

    public string ThanksForPrimeReSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankPrimeReSubMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: user.Value);
    }

    public string ThanksForPaidReSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankPaidReSubMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: user.Value);
    }

    public string ThanksForNewPaidSub(in Viewer user)
    {
        string formatString = this.GetMessage(ThankNewPaidSubMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: user.Value);
    }

    public string ThanksForGiftingMultipleSubs(in Viewer giftedBy)
    {
        string formatString = this.GetMessage(ThankGiftMultipleSubsMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: giftedBy.Value);
    }

    public string ThanksForGiftingOneSub(in Viewer giftedBy)
    {
        string formatString = this.GetMessage(ThankGiftOneSubMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: giftedBy.Value);
    }

    public string WelcomeMessage(in Viewer user)
    {
        string formatString = this.GetMessage(WelcomeMessages);

        return string.Format(provider: CultureInfo.InvariantCulture, format: formatString, arg0: user.Value);
    }

    private string GetMessage(IReadOnlyList<string> messages)
    {
        int messageIndex = this._randomNumberGenerator.Next(messages.Count);

        return messages[messageIndex];
    }
}