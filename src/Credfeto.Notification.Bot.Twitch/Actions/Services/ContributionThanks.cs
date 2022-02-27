using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ContributionThanks : MessageSenderBase, IContributionThanks
{
    public ContributionThanks(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
        : base(twitchChatMessageChannel)
    {
    }
}