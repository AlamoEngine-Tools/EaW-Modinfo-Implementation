using System;
using System.Linq;
using EawModinfo.Model.Json;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

internal class DependencyListTypeConverter : JsonConverter<IModDependencyList>
{
    public override void WriteJson(JsonWriter writer, IModDependencyList? value, JsonSerializer serializer)
    {
        if (value is null)
            throw new InvalidOperationException("dependency list property must not be null");

        if (!value.Any())
            return;

        writer.WriteStartArray();
            
        if (value.ResolveLayout != DependencyResolveLayout.ResolveRecursive)
            writer.WriteValue(value.ResolveLayout.ToString());

        foreach (var modRef in value)
            serializer.Serialize(writer, new JsonModReference(modRef), typeof(JsonModReference));

        writer.WriteEndArray();
    }

    public override IModDependencyList ReadJson(JsonReader reader, Type objectType, IModDependencyList? existingValue,
        bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.StartArray)
            throw new JsonSerializationException($"Expected array start token, but got: {reader.TokenType}");

        var dependencyList = new JsonDependencyList();
        var layout = DependencyResolveLayout.ResolveRecursive;

        var layoutIsValid = true;
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var modReference = serializer.Deserialize<JsonModReference>(reader);
                if (modReference is null)
                    throw new JsonSerializationException("Unable to parse object. Expected a ModReference type.");
                dependencyList.AddItemInternal(modReference);
                layoutIsValid = false;
            }

            if (reader.Value != null)
            {
                if (reader.Value is not string layoutString || !layoutIsValid)
                    throw new JsonSerializationException("Unexpected item in array. Expected a mod ModReference type");
                if (!Enum.TryParse<DependencyResolveLayout>(layoutString, true, out var customLayout))
                    throw new JsonSerializationException("Unable to parse the dependency layout.");
                layout = customLayout;
            }

            if (reader.TokenType == JsonToken.EndArray)
                break;
        }

        dependencyList.ResolveLayout = layout;
        return dependencyList;
    }
}