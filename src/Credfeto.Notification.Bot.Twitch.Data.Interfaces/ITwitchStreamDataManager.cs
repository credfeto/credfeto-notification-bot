using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamDataManager
{
    Task RecordStreamStartAsync(Channel channel, DateTime streamStartDate);

    Task AddChatterToStreamAsync(Channel channel, DateTime streamStartDate, User username);

    Task<bool> IsFirstMessageInStreamAsync(Channel channel, DateTime streamStartDate, User username);

    Task<bool> IsRegularChatterAsync(Channel channel, User username);

    Task<bool> UpdateFollowerMilestoneAsync(Channel channel, int followerCount);

    Task<int> RecordNewFollowerAsync(Channel channel, User username);
}