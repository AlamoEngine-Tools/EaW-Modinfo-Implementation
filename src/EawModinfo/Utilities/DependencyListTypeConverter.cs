using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AET.Modinfo.Spec;

namespace AET.Modinfo.Utilities;

internal class DependencyListTypeConverter : JsonConverter<IModDependencyList>
{
    public override IModDependencyList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonDependencyListTypeConverter.ReadInternal(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, IModDependencyList value, JsonSerializerOptions options)
    {
        JsonDependencyListTypeConverter.WriteCore(writer, value, options);
    }
}