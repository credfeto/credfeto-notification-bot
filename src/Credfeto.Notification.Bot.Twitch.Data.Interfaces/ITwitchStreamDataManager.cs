using System;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamDataManager
{
    Task RecordStreamStartAsync(string channel, DateTime streamStartDate);
}