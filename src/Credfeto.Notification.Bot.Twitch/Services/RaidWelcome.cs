using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class RaidWelcome : MessageSenderBase, IRaidWelcome
{
    public RaidWelcome(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
        : base(twitchChatMessageChannel)
    {
    }

    public async Task IssueRaidWelcomeAsync(string channel, string raider, CancellationToken cancellationToken)
    {
        const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.SendMessageAsync(channel: channel, message: raidWelcome, cancellationToken: cancellationToken);
        await this.SendMessageAsync(channel: channel, $"Thanks @{raider} for the raid", cancellationToken: cancellationToken);
        await this.SendMessageAsync(channel: channel, $"Check out https://www.twitch.tv/{raider}", cancellationToken: cancellationToken);
    }
}