using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Resources;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class RaidWelcome : IRaidWelcome
{
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public RaidWelcome(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
    {
        this._twitchChatMessageChannel = twitchChatMessageChannel;
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

    private ValueTask SendMessageAsync(string channel, string message, in CancellationToken cancellationToken)
    {
        TwitchChatMessage toSend = new(channel: channel, message: message);

        return this._twitchChatMessageChannel.PublishAsync(message: toSend, cancellationToken: cancellationToken);
    }
}