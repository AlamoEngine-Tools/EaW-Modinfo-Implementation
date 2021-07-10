using System;
using EawModinfo.Model;
using EawModinfo.Model.Json;
using Newtonsoft.Json;

namespace EawModinfo.Utilities
{
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
        }
    }
}