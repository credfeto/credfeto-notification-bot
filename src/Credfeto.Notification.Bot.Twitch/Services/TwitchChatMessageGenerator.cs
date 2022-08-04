using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChatMessageGenerator : ITwitchChatMessageGenerator
{
    public string ThanksForBits(in Viewer giftedBy)
    {
        return $"Thanks @{giftedBy} for the bits";
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
}