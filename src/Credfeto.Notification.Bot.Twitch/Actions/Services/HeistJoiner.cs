using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

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