using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchStreamDataManager : ITwitchStreamDataManager
{
    private readonly IObjectBuilder<TwitchChatterEntity, TwitchChatter> _chatterBuilder;
    private readonly IDatabase _database;

    public TwitchStreamDataManager(IDatabase database, IObjectBuilder<TwitchChatterEntity, TwitchChatter> chatterBuilder)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
        this._chatterBuilder = chatterBuilder ?? throw new ArgumentNullException(nameof(chatterBuilder));
    }

    public Task RecordStreamStartAsync(string channel, DateTime streamStartDate)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.stream_insert", new { channel_ = channel, start_date_ = streamStartDate });
    }

    public Task AddChatterToStreamAsync(string channel, DateTime streamStartDate, string username)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.stream_chatter_insert", new { channel_ = channel, start_date_ = streamStartDate, chat_user_ = username });
    }

    public async Task<bool> IsFirstMessageInStreamAsync(string channel, DateTime streamStartDate, string username)
    {
        TwitchChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._chatterBuilder,
                                                                                storedProcedure: "twitch.stream_chatter_get",
                                                                                new { channel_ = channel, start_date_ = streamStartDate, chat_user_ = username });

        return chatted == null;
    }
}