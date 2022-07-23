using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;

namespace EawModinfo.Utilities;

<<<<<<< HEAD
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
=======
internal class SteamDataTypeConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<JsonSteamData>(reader);
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
>>>>>>> to c# 10 namespaces
    }
}