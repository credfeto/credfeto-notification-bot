using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamRaidedNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer Raider = Viewer.FromString(nameof(Raider));

    private readonly INotificationHandler<TwitchStreamRaided> _notificationHandler;
    private readonly IRaidWelcome _raidWelcome;

    public TwitchStreamRaidedNotificationHandlerTests()
    {
        this._raidWelcome = GetSubstitute<IRaidWelcome>();

        this._notificationHandler = new TwitchStreamRaidedNotificationHandler(raidWelcome: this._raidWelcome, this.GetTypedLogger<TwitchStreamRaidedNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: Streamer, raider: Raider, viewerCount: 254643), cancellationToken: CancellationToken.None);

        await this._raidWelcome.Received(1)
                  .IssueRaidWelcomeAsync(streamer: Streamer, raider: Raider, Arg.Any<CancellationToken>());
    }
}