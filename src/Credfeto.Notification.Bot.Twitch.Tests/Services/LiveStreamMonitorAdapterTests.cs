using System;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using NSubstitute;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class LiveStreamMonitorAdapterTests : TestBase
{
    private readonly ILiveStreamMonitor _liveStreamMonitor;

    public LiveStreamMonitorAdapterTests()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
        );

        this._liveStreamMonitor = new LiveStreamMonitorAdapter(options);
    }

    [Fact]
    public void StreamOnlineHandlerCanBeAddedAndRemoved()
    {
        Exception? exception = Record.Exception(() =>
        {
            this._liveStreamMonitor.OnStreamOnline += OnStreamOnline;
            this._liveStreamMonitor.OnStreamOnline -= OnStreamOnline;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void StreamOfflineHandlerCanBeAddedAndRemoved()
    {
        Exception? exception = Record.Exception(() =>
        {
            this._liveStreamMonitor.OnStreamOffline += OnStreamOffline;
            this._liveStreamMonitor.OnStreamOffline -= OnStreamOffline;
        });

        Assert.Null(exception);
    }

    [Fact]
    public void SetChannelsByNameAcceptsChannels()
    {
        Exception? exception = Record.Exception(() => this._liveStreamMonitor.SetChannelsByName(["teststreamer1"]));

        Assert.Null(exception);
    }

    private static void OnStreamOnline(object? sender, OnStreamOnlineArgs e)
    {
        // Intentionally empty: only the subscription plumbing is under test.
    }

    private static void OnStreamOffline(object? sender, OnStreamOfflineArgs e)
    {
        // Intentionally empty: only the subscription plumbing is under test.
    }
}
