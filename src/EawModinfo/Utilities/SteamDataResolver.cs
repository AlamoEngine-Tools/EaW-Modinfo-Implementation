using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EawModinfo.Utilities;

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
internal class NullToEmptyStringSerializerConverter : JsonConverter<string?>
{
    public override bool HandleNull => true;

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string);
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ?? "");
<<<<<<< HEAD
=======
internal class SteamDataResolver : DefaultContractResolver
=======
internal class NullToEmptyStringSerializerConverter : JsonConverter<string?>
>>>>>>> System text json (#134)
{
    public override bool HandleNull => true;

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string);
    }

    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

<<<<<<< HEAD
        public void SetValue(object target, object? value)
        {
            _provider?.SetValue(target, value);
        }
>>>>>>> to c# 10 namespaces
=======
    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ?? "");
>>>>>>> System text json (#134)
=======
>>>>>>> b7dafff0b6609730c7665be9f05a50996f5a0bbd
    }
}