using System.Data;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;

public sealed class TwitchChatterBuilder : IObjectBuilder<TwitchChatterEntity, TwitchChatter>
{
    public TwitchChatter Build(TwitchChatterEntity source)
    {
        return new(source.Channel ?? throw new DataException("Unknown Channel"),
                   streamStartDate: source.Start_Date,
                   source.Chat_User ?? throw new DataException("Unknown Chat_User"),
                   firstMessage: source.First_Message_Date);
    }
}