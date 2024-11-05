using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using EawModinfo.Model.Json;
using EawModinfo.Model.Json.Schema;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

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

    public static T Parse<T>(string data)
    {
        if (string.IsNullOrEmpty(data))
            throw new ModinfoParseException("No input data.");
        try
        {
            var jsonNode = JsonNode.Parse(data, null, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });

            ModInfoJsonSchema.Evaluate<T>(jsonNode);

            var parseResult = jsonNode.Deserialize<T>(SerializerOptions);
            if (parseResult is null)
                throw new ModinfoParseException($"Unable to parse input '{data}' to {typeof(T).Name}. Unknown Error!");
            return parseResult;
        }
        catch (JsonException cause)
        {
            throw new ModinfoParseException(cause.Message, cause);
        }
    }
}