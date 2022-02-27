using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Resources;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class HeistJoiner : MessageSenderBase, IHeistJoiner
{
    public HeistJoiner(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
        : base(twitchChatMessageChannel)
    {
    }

    public Task JoinHeistAsync(string channel, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;

        //return base.SendMessageAsync(channel: channel, message: "!heist all")''
    }
}