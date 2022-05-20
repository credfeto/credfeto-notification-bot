using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Discord.Commands;

namespace Credfeto.Notification.Bot.Discord.Commands;

public sealed class PingModule : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary(text: "Shows the status of the service")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public Task PingAsync()
    {
        return this.ReplyAsync("pong!");
    }
}