using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using EawModinfo.Spec.Steam;

namespace EawModinfo.Utilities;

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
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
<<<<<<< HEAD
=======
internal class SteamDataTypeConverter : JsonConverter
=======
internal class SteamDataTypeConverter : JsonConverter<ISteamData>
>>>>>>> System text json (#134)
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
<<<<<<< HEAD
        return true;
>>>>>>> to c# 10 namespaces
=======
        JsonSerializer.Serialize(writer, new JsonSteamData(value), options);
>>>>>>> System text json (#134)
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
    }
}