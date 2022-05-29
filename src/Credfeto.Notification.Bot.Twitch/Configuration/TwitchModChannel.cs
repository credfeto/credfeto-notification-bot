using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchModChannel
{
    [JsonConstructor]
    public TwitchModChannel(string channelName, TwitchChannelShoutout shoutOuts, TwitchChannelRaids raids, TwitchChannelThanks thanks, TwitchChannelMileStone mileStones, TwitchChannelWelcome welcome)
    {
        this.ChannelName = channelName;
        this.ShoutOuts = shoutOuts;
        this.Raids = raids;
        this.Thanks = thanks;
        this.MileStones = mileStones;
        this.Welcome = welcome;
    }

    public string ChannelName { get; }

    public TwitchChannelShoutout ShoutOuts { get; }

    public TwitchChannelRaids Raids { get; }

    public TwitchChannelThanks Thanks { get; }

    public TwitchChannelMileStone MileStones { get; }

    public TwitchChannelWelcome Welcome { get; }
}