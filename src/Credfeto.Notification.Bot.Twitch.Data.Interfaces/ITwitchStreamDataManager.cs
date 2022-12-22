using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamDataManager
{
    Task RecordStreamStartAsync(Streamer streamer, DateTimeOffset streamStartDate);

    Task AddChatterToStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username);

    Task<bool> IsFirstMessageInStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username);

    Task<bool> IsRegularChatterAsync(Streamer streamer, Viewer username);

    Task<bool> UpdateFollowerMilestoneAsync(Streamer streamer, int followerCount);

    Task<int> RecordNewFollowerAsync(Streamer streamer, Viewer username);

    Task<StreamSettings?> GetSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate);

    Task UpdateSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, StreamSettings settings);
}