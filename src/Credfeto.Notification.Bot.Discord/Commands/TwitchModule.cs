using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Discord;
using Discord.Commands;

namespace Credfeto.Notification.Bot.Discord.Commands;

public sealed class TwitchModule : ModuleBase<SocketCommandContext>
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly IUserInfoService _userInfoService;

    public TwitchModule(ITwitchChannelManager twitchChannelManager, IUserInfoService userInfoService, IChannelFollowCount channelFollowCount)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._channelFollowCount = channelFollowCount ?? throw new ArgumentNullException(nameof(channelFollowCount));
    }

    [Command("status")]
    [Summary(text: "Shows the status of a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task StatusAsync(string streamer)
    {
        Streamer s = Streamer.FromString(streamer);
        TwitchUser? user = await this._userInfoService.GetUserAsync(userName: s, cancellationToken: CancellationToken.None);

        if (user == null)
        {
            await this.ReplyAsync($"Streamer {s} is not registered");

            return;
        }

        int followCount = await this._channelFollowCount.GetCurrentFollowerCountAsync(streamer: s, cancellationToken: CancellationToken.None);

        ITwitchChannelState streamerStatus = this._twitchChannelManager.GetStreamer(s);

        string title = $"{streamerStatus.Streamer} Current Status.";

        Embed embed = new EmbedBuilder().WithColor(streamerStatus.IsOnline
                                                       ? Color.Green
                                                       : Color.DarkGrey)
                                        .WithCurrentTimestamp()
                                        .WithTitle(title)
                                        .WithUrl($"https://twitch.tv/{streamerStatus.Streamer}")
                                        .AddField(name: "Online", value: streamerStatus.IsOnline)
                                        .AddField(name: "Followers", value: followCount)
                                        .AddField(name: "Welcome regulars", value: streamerStatus.Settings.ChatWelcomesEnabled)
                                        .AddField(name: "Welcome raiders", value: streamerStatus.Settings.RaidWelcomesEnabled)
                                        .AddField(name: "Thank sub/bit", value: streamerStatus.Settings.ThanksEnabled)
                                        .AddField(name: "Shoutout", value: streamerStatus.Settings.ShoutOutsEnabled)
                                        .AddField(name: "Milestone Announcements", value: streamerStatus.Settings.AnnounceMilestonesEnabled)
                                        .Build();

        await this.ReplyAsync(embed: embed);
    }

    [Command("welcome")]
    [Summary(text: "Enables or disables the welcome messages for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task WelcomeWaggonAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideWelcomes(enabled))
        {
            await channel.SaveStreamSettingsAsync(CancellationToken.None);
            await this.ReplyAsync($"Welcomes for regular {streamer} chatters are now {enabled.AsEnabledDisabled()}");
        }
        else
        {
            bool status = channel.Settings.ChatWelcomesEnabled;
            await this.ReplyAsync($"Welcomes for regular {streamer} chatters unchanged. Status: {status.AsEnabledDisabled()}");
        }
    }

    [Command("raid")]
    [Summary(text: "Enables or disables the raid welcome messages for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task RaidWelcomesAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideRaidWelcomes(enabled))
        {
            await channel.SaveStreamSettingsAsync(CancellationToken.None);
            await this.ReplyAsync($"Raid Welcomes for regular {streamer} chatters are now {enabled.AsEnabledDisabled()}");
        }
        else
        {
            bool status = channel.Settings.RaidWelcomesEnabled;
            await this.ReplyAsync($"Raid Welcomes for regular {streamer} chatters unchanged. Status: {status.AsEnabledDisabled()}");
        }
    }

    [Command("thanks")]
    [Summary(text: "Enables or disables thanks for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task ThanksAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideThanks(enabled))
        {
            await channel.SaveStreamSettingsAsync(CancellationToken.None);
            await this.ReplyAsync($"Thanks for {streamer} gifts  are now {enabled.AsEnabledDisabled()}");
        }
        else
        {
            bool status = channel.Settings.ThanksEnabled;
            await this.ReplyAsync($"Thanks for {streamer} gifts unchanged. Status: {status.AsEnabledDisabled()}");
        }
    }

    [Command("milestones")]
    [Summary(text: "Enables or disables announcing milestones for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task MilestonesEnabledAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideMilestonesEnabled(enabled))
        {
            await channel.SaveStreamSettingsAsync(CancellationToken.None);
            await this.ReplyAsync($"Milestone announcements for {streamer} gifts  are now {enabled.AsEnabledDisabled()}");
        }
        else
        {
            bool status = channel.Settings.AnnounceMilestonesEnabled;
            await this.ReplyAsync($"Milestone announcements for {streamer} gifts unchanged. Status: {status.AsEnabledDisabled()}");
        }
    }

    [Command("shoutouts")]
    [Summary(text: "Enables or disables auto shout-outs for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task ShoutoutsEnabledAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideShoutOuts(enabled))
        {
            await channel.SaveStreamSettingsAsync(CancellationToken.None);
            await this.ReplyAsync($"Auto-Shoutouts for {streamer} gifts  are now {enabled.AsEnabledDisabled()}");
        }
        else
        {
            bool status = channel.Settings.ShoutOutsEnabled;
            await this.ReplyAsync($"Auto-Shoutouts for {streamer} gifts unchanged. Status: {status.AsEnabledDisabled()}");
        }
    }

    [Command("automod")]
    [Summary(text: "Enables or disables automod for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task AutoModAsync(string streamer, bool enabled = true)
    {
        await this.StatusAsync(streamer);
        await this.WelcomeWaggonAsync(streamer: streamer, enabled: enabled);
        await this.RaidWelcomesAsync(streamer: streamer, enabled: enabled);
        await this.ThanksAsync(streamer: streamer, enabled: enabled);
        await this.MilestonesEnabledAsync(streamer: streamer, enabled: enabled);
        await this.ShoutoutsEnabledAsync(streamer: streamer, enabled: enabled);
        await this.StatusAsync(streamer);
    }
}