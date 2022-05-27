using System;
using System.Buffers;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Server.ServiceStartup.Configuration;

public static class TypeConfigurationExtensions
{
    public static IServiceCollection WithConfiguration<TSettings>(this IServiceCollection services, IConfigurationRoot configurationRoot, string key, JsonSerializerContext jsonSerializerContext)
        where TSettings : class
    {
        IConfigurationSection section = configurationRoot.GetSection(key);

        if (jsonSerializerContext.GetTypeInfo(typeof(TSettings)) is not JsonTypeInfo<TSettings> typeInfo)
        {
            throw new JsonException($"No Json Type Info for {typeof(TSettings).FullName}");
        }

        string result = ToJson(section: section, jsonSerializerOptions: jsonSerializerContext.Options);

        Console.WriteLine(result);

        TSettings settings = JsonSerializer.Deserialize(json: result, jsonTypeInfo: typeInfo) ?? throw new JsonException("Could not deserialize options");

        IOptions<TSettings> toRegister = Options.Create(settings);

        return services.AddSingleton(toRegister);
    }

    private static string ToJson(this IConfigurationSection section, JsonSerializerOptions jsonSerializerOptions)
    {
        ArrayBufferWriter<byte> bufferWriter = new(jsonSerializerOptions.DefaultBufferSize);

        using (Utf8JsonWriter jsonWriter = new(bufferWriter: bufferWriter, new() { Encoder = jsonSerializerOptions.Encoder, Indented = jsonSerializerOptions.WriteIndented, SkipValidation = false }))
        {
            jsonWriter.WriteStartObject();
            Serialize(config: section, writer: jsonWriter, jsonSerializerOptions: jsonSerializerOptions);
            jsonWriter.WriteEndObject();
        }

        return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
    }

    private static void Serialize(this IConfiguration config, Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions)
    {
        bool written = false;

        foreach (IConfigurationSection child in config.GetChildren())
        {
            written = true;

            if (child.Path.EndsWith(value: ":0", comparisonType: StringComparison.Ordinal))
            {
                writer.WriteStartArray();

                foreach (IConfigurationSection? arrayChild in config.GetChildren())
                {
                    Serialize(config: arrayChild, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
                }

                writer.WriteEndArray();

                return;
            }

            if (child.GetChildren()
                     .Any())
            {
                writer.WritePropertyName(ConvertName(jsonSerializerOptions: jsonSerializerOptions, name: child.Key));
                writer.WriteStartObject();
                Serialize(config: child, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
                writer.WriteEndObject();
            }
            else
            {
                Serialize(config: child, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
            }
        }

        if (!written && config is IConfigurationSection section)
        {
            SerializeSimpleProperty(writer: writer, jsonSerializerOptions: jsonSerializerOptions, section: section);
        }
    }

    private static void SerializeSimpleProperty(Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions, IConfigurationSection section)
    {
        if (bool.TryParse(value: section.Value, out bool boolean))
        {
            writer.WriteBoolean(ConvertName(jsonSerializerOptions: jsonSerializerOptions, name: section.Key), value: boolean);

            return;
        }

        if (decimal.TryParse(s: section.Value, out decimal real))
        {
            writer.WriteNumber(ConvertName(jsonSerializerOptions: jsonSerializerOptions, name: section.Key), value: real);

            return;
        }

        if (long.TryParse(s: section.Value, out long integer))
        {
            writer.WriteNumber(ConvertName(jsonSerializerOptions: jsonSerializerOptions, name: section.Key), value: integer);

            return;
        }

        writer.WriteString(ConvertName(jsonSerializerOptions: jsonSerializerOptions, name: section.Key), value: section.Value);
    }

    private static string ConvertName(JsonSerializerOptions jsonSerializerOptions, string name)
    {
        return jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(name) ?? name;
    }
}