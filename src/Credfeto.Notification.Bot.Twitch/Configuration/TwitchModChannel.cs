using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchModChannel
{
    [JsonConstructor]
    public TwitchModChannel(string channelName,
                            TwitchChannelShoutout? shoutOuts,
                            TwitchChannelRaids? raids,
                            TwitchChannelThanks? thanks,
                            TwitchChannelMileStone? mileStones,
                            TwitchChannelWelcome? welcome)
    {
        this.ChannelName = channelName;
        this.ShoutOuts = shoutOuts ?? new TwitchChannelShoutout(enabled: false, new());
        this.Raids = raids ?? new TwitchChannelRaids(enabled: false, immediate: null, calmDown: null);
        this.Thanks = thanks ?? new TwitchChannelThanks(enabled: false);
        this.MileStones = mileStones ?? new TwitchChannelMileStone(enabled: false);
        this.Welcome = welcome ?? new TwitchChannelWelcome(false);
    }

    public string ChannelName { get; }

    public TwitchChannelShoutout ShoutOuts { get; }

    public TwitchChannelRaids Raids { get; }

    public TwitchChannelThanks Thanks { get; }

    public TwitchChannelMileStone MileStones { get; }

    public TwitchChannelWelcome Welcome { get; }
}