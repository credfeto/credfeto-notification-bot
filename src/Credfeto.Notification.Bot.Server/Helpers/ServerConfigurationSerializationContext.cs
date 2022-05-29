using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Credfeto.Notification.Bot.Database.Pgsql;
using Credfeto.Notification.Bot.Discord;
using Credfeto.Notification.Bot.Twitch.Configuration;

namespace Credfeto.Notification.Bot.Server.Helpers;

[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Required for JsonSerializerContext")]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization | JsonSourceGenerationMode.Metadata,
                             PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
                             DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                             WriteIndented = false,
                             IncludeFields = false)]
[JsonSerializable(typeof(DiscordBotOptions))]
[JsonSerializable(typeof(TwitchBotOptions))]
[JsonSerializable(typeof(PgsqlServerConfiguration))]
internal sealed partial class ServerConfigurationSerializationContext : JsonSerializerContext
{
}