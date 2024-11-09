using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using EawModinfo.Model.Json;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

internal class JsonDependencyListTypeConverter : JsonConverter<JsonDependencyList>
{
    public override JsonDependencyList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return ReadInternal(ref reader, typeToConvert, options);
    }

    public override void Write(Utf8JsonWriter writer, JsonDependencyList value, JsonSerializerOptions options)
    {
        WriteCore(writer, value, options);
    }

    internal static JsonDependencyList ReadInternal(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected array start token, but got: {reader.TokenType}");

        var dependencyList = new JsonDependencyList();
        var layout = DependencyResolveLayout.ResolveRecursive;

        var mayHaveLayoutValue = true;
        var hasReference = false;
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var modReference = JsonSerializer.Deserialize<JsonModReference>(ref reader, options);
                if (modReference is null)
                    throw new JsonException("Unable to parse object. Expected a ModReference type.");
                dependencyList.AddItemInternal(modReference);
                mayHaveLayoutValue = false;
                hasReference = true;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var valueString = reader.GetString();
                if (valueString is null || !mayHaveLayoutValue)
                    throw new JsonException("Unexpected item in array. Expected a mod ModReference type");
                if (!Enum.TryParse<DependencyResolveLayout>(valueString, true, out var customLayout))
                    throw new JsonException("Unable to parse the dependency layout.");
                layout = customLayout;
                mayHaveLayoutValue = false;
            }

            if (reader.TokenType == JsonTokenType.EndArray)
                break;
        }

        if (!hasReference)
            throw new JsonException("dependency list is missing at least one modreference.");

        dependencyList.ResolveLayout = layout;
        return dependencyList;
    }

    internal static void WriteCore(Utf8JsonWriter writer, IModDependencyList value, JsonSerializerOptions options)
    {
        if (!value.Any())
            return;

        writer.WriteStartArray();

        if (value.ResolveLayout != DependencyResolveLayout.ResolveRecursive)
            writer.WriteStringValue(value.ResolveLayout.ToString());

        foreach (var modRef in value)
            JsonSerializer.Serialize(writer, new JsonModReference(modRef), options);

        writer.WriteEndArray();
    }
}