using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Credfeto.Database.Pgsql;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Setup;

[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Required for JsonSerializerContext")]
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization | JsonSourceGenerationMode.Metadata,
                             PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
                             DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                             WriteIndented = false,
                             IncludeFields = false)]
[JsonSerializable(typeof(PgsqlServerConfiguration))]
internal sealed partial class DatabaseConfigurationSerializationContext : JsonSerializerContext
{
}