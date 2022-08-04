using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchChatMessageGenerator
{
    string ThanksForBits(in Viewer giftedBy, int bitsGiven);

    string ThanksForNewPrimeSub(in Viewer user);

    string ThanksForPrimeReSub(in Viewer user);

    string ThanksForPaidReSub(in Viewer user);

    string ThanksForNewPaidSub(in Viewer user);

    string ThanksForGiftingMultipleSubs(in Viewer giftedBy);

    string ThanksForGiftingOneSub(in Viewer giftedBy);

    string WelcomeMessage(in Viewer user);
}