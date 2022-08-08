using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamDataManager
{
    Task RecordStreamStartAsync(Streamer streamer, DateTime streamStartDate);

    Task AddChatterToStreamAsync(Streamer streamer, DateTime streamStartDate, Viewer username);

    Task<bool> IsFirstMessageInStreamAsync(Streamer streamer, DateTime streamStartDate, Viewer username);

    Task<bool> IsRegularChatterAsync(Streamer streamer, Viewer username);

    Task<bool> UpdateFollowerMilestoneAsync(Streamer streamer, int followerCount);

    Task<int> RecordNewFollowerAsync(Streamer streamer, Viewer username);
}