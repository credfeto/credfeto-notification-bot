using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamDataManager
{
    ValueTask RecordStreamStartAsync(Streamer streamer, DateTimeOffset streamStartDate, CancellationToken cancellationToken);

    ValueTask AddChatterToStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username, CancellationToken cancellationToken);

    ValueTask<bool> IsFirstMessageInStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username, CancellationToken cancellationToken);

    ValueTask<bool> IsRegularChatterAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken);

    ValueTask<bool> UpdateFollowerMilestoneAsync(Streamer streamer, int followerCount, CancellationToken cancellationToken);

    ValueTask<int> RecordNewFollowerAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken);

    ValueTask<StreamSettings?> GetSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, CancellationToken cancellationToken);

    ValueTask UpdateSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, StreamSettings settings, CancellationToken cancellationToken);
}