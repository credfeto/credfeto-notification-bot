using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Discord;
using Discord.Commands;

namespace Credfeto.Notification.Bot.Discord.Commands;

/// <summary>
///     Twitch Command module
/// </summary>
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

    /// <summary>
    ///     Gets the status of a streamer
    /// </summary>
    [Command("status")]
    [Summary(text: "Shows the status of a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task StatusAsync(string streamer)
    {
        Streamer s = Streamer.FromString(streamer);
        TwitchUser? user = await this._userInfoService.GetUserAsync(s);

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

    /// <summary>
    ///     Gets the status of a streamer
    /// </summary>
    [Command("welcome")]
    [Summary(text: "Enables or disables the welcome messages for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task WelcomeWaggonAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideWelcomes(enabled))
        {
            await this.ReplyAsync($"Welcomes for regular {streamer} chatters are now {(enabled ? "enabled" : "disabled")}");
        }
        else
        {
            await this.ReplyAsync($"Welcomes for regular {streamer} chatters unchanged. Status: {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    ///     Gets the status of a streamer
    /// </summary>
    [Command("raid")]
    [Summary(text: "Enables or disables the raid welcome messages for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task RaidWelcomesAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideRaidWelcomes(enabled))
        {
            await this.ReplyAsync($"Raid Welcomes for regular {streamer} chatters are now {(enabled ? "enabled" : "disabled")}");
        }
        else
        {
            await this.ReplyAsync($"Raid Welcomes for regular {streamer} chatters unchanged. Status: {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    ///     Enables/disables thanks for gifts or subs to streamer
    /// </summary>
    [Command("thanks")]
    [Summary(text: "Enables or disables thanks for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task ThanksAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideThanks(enabled))
        {
            await this.ReplyAsync($"Thanks for {streamer} gifts  are now {(enabled ? "enabled" : "disabled")}");
        }
        else
        {
            await this.ReplyAsync($"Thanks for {streamer} gifts unchanged. Status: {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    ///     Enables or disables announcing milestones for a streamer
    /// </summary>
    [Command("milestones")]
    [Summary(text: "Enables or disables announcing milestones for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task MilestonesEnabledAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideMilestonesEnabled(enabled))
        {
            await this.ReplyAsync($"Milestone announcements for {streamer} gifts  are now {(enabled ? "enabled" : "disabled")}");
        }
        else
        {
            await this.ReplyAsync($"Milestone announcements for {streamer} gifts unchanged. Status: {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    ///     Enables or disables auto-shout-outs for a streamer
    /// </summary>
    [Command("shoutouts")]
    [Summary(text: "Enables or disables auto shout-outs for a streamer")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Add unit tests")]
    public async Task ShoutoutsEnabledAsync(string streamer, bool enabled = true)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(Streamer.FromString(streamer));

        if (channel.Settings.OverrideShoutOuts(enabled))
        {
            await this.ReplyAsync($"Auto-Shoutouts for {streamer} gifts  are now {(enabled ? "enabled" : "disabled")}");
        }
        else
        {
            await this.ReplyAsync($"Auto-Shoutouts for {streamer} gifts unchanged. Status: {(enabled ? "enabled" : "disabled")}");
        }
    }

    /// <summary>
    ///     Turns automod on/off for a streamer
    /// </summary>
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