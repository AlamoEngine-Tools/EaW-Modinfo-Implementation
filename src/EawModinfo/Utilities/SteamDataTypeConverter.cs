using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;

namespace EawModinfo.Utilities;

internal class SteamDataTypeConverter : JsonConverter<ISteamData>
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override ISteamData? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new SteamData(JsonSerializer.Deserialize<JsonSteamData>(ref reader, options)!);
    }

    public override void Write(Utf8JsonWriter writer, ISteamData value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, new JsonSteamData(value), options);
    }
}