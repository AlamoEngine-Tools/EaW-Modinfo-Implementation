using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using AET.Modinfo.Model.Json;
using AET.Modinfo.Model.Json.Schema;
using AET.Modinfo.Spec;

namespace AET.Modinfo.Utilities;

internal static class ParseUtility
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers = { DefaultValueModifier }
        }
    };

    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip
    };

    private static void DefaultValueModifier(JsonTypeInfo obj)
    {
        if (typeof(JsonModinfoData).IsAssignableFrom(obj.Type))
        {
            var propsToIgnoreWhenEmpty = obj.Properties.Where(p =>
                p.Name is "custom" or "languages" or "dependencies");
            foreach (var propertyInfo in propsToIgnoreWhenEmpty)
            {
                propertyInfo.ShouldSerialize = ShouldSerializeModinfoData;
                continue;

                bool ShouldSerializeModinfoData(object _, object? value)
                {
                    if (value is IModDependencyList dependencyList)
                        return dependencyList.Count > 0;
                    if (value is HashSet<JsonLanguageInfo> languages)
                        return languages.Count > 0;
                    if (value is IDictionary<string, object> custom)
                        return custom.Count > 0;
                    return false;
                }
            }
        }
        else if (typeof(JsonSteamData).IsAssignableFrom(obj.Type))
        {
            var emptyStringPropsWhenNull = obj.Properties.Where(p =>
                p.Name is "metadata" or "description" or "previewfile");
            foreach (var propertyInfo in emptyStringPropsWhenNull)
                propertyInfo.CustomConverter = new NullToEmptyStringSerializerConverter();
        }
    }

    public static T Parse<T>(Stream dataStream)
    {
        try
        { 
            var jsonNode = JsonNode.Parse(dataStream, null, DocumentOptions);
            var result = ParseCore<T>(jsonNode);

            if (result is null)
                throw new ModinfoParseException($"Unable to parse input from stream to {typeof(T).Name}. Unknown Error!");
            return result;
        }
        catch (JsonException cause)
        {
            throw new ModinfoParseException(cause.Message, cause);
        }
    }

    public static T Parse<T>(string data)
    {
        try
        {
            var jsonNode = JsonNode.Parse(data, null, DocumentOptions);
            var parseResult = ParseCore<T>(jsonNode);
            if (parseResult is null)
                throw new ModinfoParseException($"Unable to parse input '{data}' to {typeof(T).Name}. Unknown Error!");
            return parseResult;
        }
        catch (JsonException cause)
        {
            throw new ModinfoParseException(cause.Message, cause);
        }
    }

    public static T? ParseCore<T>(JsonNode? jsonData)
    {
        if (jsonData == null)
            throw new ModinfoParseException("Unable to parse input to JSON Node");

        ModInfoJsonSchema.Evaluate<T>(jsonData);
        return jsonData.Deserialize<T>(SerializerOptions);
    }
}