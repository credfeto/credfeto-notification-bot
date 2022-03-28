using System;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamDataManager
{
    Task RecordStreamStartAsync(string channel, DateTime streamStartDate);

    Task AddChatterToStreamAsync(string channel, DateTime streamStartDate, string username);

    Task<bool> IsFirstMessageInStreamAsync(string channel, DateTime streamStartDate, string username);

    Task<bool> IsRegularChatterAsync(string channel, string username);

    Task<bool> UpdateFollowerMilestoneAsync(string channel, int followerCount);

    Task<int> RecordNewFollowerAsync(string channelName, string username);
}