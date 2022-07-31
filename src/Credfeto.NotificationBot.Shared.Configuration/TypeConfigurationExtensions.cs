using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Credfeto.NotificationBot.Shared.Configuration;

public static class TypeConfigurationExtensions
{
    public static IServiceCollection WithConfiguration<TSettings>(this IServiceCollection services,
                                                                  IConfigurationRoot configurationRoot,
                                                                  string key,
                                                                  JsonSerializerContext jsonSerializerContext)
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

        using (Utf8JsonWriter jsonWriter = new(bufferWriter: bufferWriter,
                                               new() { Encoder = jsonSerializerOptions.Encoder, Indented = jsonSerializerOptions.WriteIndented, SkipValidation = false }))
        {
            SerializeObject(config: section, writer: jsonWriter, jsonSerializerOptions: jsonSerializerOptions);
        }

        return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
    }

    private static void SerializeObject(this IConfigurationSection config, Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions)
    {
        using (new JsonObjectWriter(writer))
        {
            IReadOnlyList<IConfigurationSection> children = config.GetChildren()
                                                                  .ToArray();

            foreach (IConfigurationSection section in children)
            {
                SerialiseSection(section: section, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
            }
        }
    }

    private static void SerialiseSection(IConfigurationSection section, Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions)
    {
        IConfigurationSection? firstChild = section.GetChildren()
                                                   .FirstOrDefault();

        if (IsFirstArrayElement(section))
        {
            throw new ArgumentOutOfRangeException(nameof(section), actualValue: section.Path, message: "Cannot write array property name");
        }

        writer.WritePropertyName(ConvertName(jsonSerializerOptions: jsonSerializerOptions, name: section.Key));

        if (firstChild != null)
        {
            if (IsFirstArrayElement(firstChild))
            {
                SerialiseArray(config: section, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
            }
            else
            {
                SerializeObject(config: section, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
            }
        }
        else
        {
            SerialiseTypedValue(configItem: section, writer: writer);
        }
    }

    private static void SerialiseArray(IConfigurationSection config, Utf8JsonWriter writer, JsonSerializerOptions jsonSerializerOptions)
    {
        using (new JsonArrayWriter(writer))
        {
            IReadOnlyList<IConfigurationSection> children = config.GetChildren()
                                                                  .ToArray();

            foreach (IConfigurationSection section in children)
            {
                if (section.GetChildren()
                           .Any())
                {
                    SerializeObject(config: section, writer: writer, jsonSerializerOptions: jsonSerializerOptions);
                }
                else
                {
                    SerialiseTypedValue(configItem: section, writer: writer);
                }
            }
        }
    }

    private static void SerialiseTypedValue(IConfigurationSection configItem, Utf8JsonWriter writer)
    {
        if (configItem.Value == null)
        {
            writer.WriteNullValue();

            return;
        }

        if (bool.TryParse(value: configItem.Value, out bool boolean))
        {
            writer.WriteBooleanValue(value: boolean);

            return;
        }

        if (decimal.TryParse(s: configItem.Value, out decimal real))
        {
            writer.WriteNumberValue(value: real);

            return;
        }

        if (long.TryParse(s: configItem.Value, out long integer))
        {
            writer.WriteNumberValue(value: integer);

            return;
        }

        writer.WriteStringValue(value: configItem.Value);
    }

    private static string ConvertName(JsonSerializerOptions jsonSerializerOptions, string name)
    {
        return jsonSerializerOptions.PropertyNamingPolicy?.ConvertName(name) ?? name;
    }

    private static bool IsFirstArrayElement(IConfigurationSection section)
    {
        return section.Path.EndsWith(value: ":0", comparisonType: StringComparison.Ordinal);
    }

    private sealed class JsonObjectWriter : IDisposable
    {
        private readonly Utf8JsonWriter _writer;

        public JsonObjectWriter(Utf8JsonWriter writer)
        {
            this._writer = writer;
            this._writer.WriteStartObject();
        }

        public void Dispose()
        {
            this._writer.WriteEndObject();
        }
    }

    private sealed class JsonArrayWriter : IDisposable
    {
        private readonly Utf8JsonWriter _writer;

        public JsonArrayWriter(Utf8JsonWriter writer)
        {
            this._writer = writer;
            this._writer.WriteStartArray();
        }

        public void Dispose()
        {
            this._writer.WriteEndArray();
        }
    }
}