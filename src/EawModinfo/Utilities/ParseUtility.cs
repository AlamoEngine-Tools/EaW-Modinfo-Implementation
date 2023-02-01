using System.Collections;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using EawModinfo.Model.Json;
using Json.Schema;

namespace EawModinfo.Utilities;

internal static class ParseUtility
{
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> System text json (#134)
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
                propertyInfo.ShouldSerialize = (_, value) => value is ICollection { Count: > 0 };
        }
        else if (typeof(JsonSteamData).IsAssignableFrom(obj.Type))
        {
            var emptyStringPropsWhenNull = obj.Properties.Where(p =>
                p.Name is "metadata" or "description" or "previewfile");
            foreach (var propertyInfo in emptyStringPropsWhenNull)
                propertyInfo.CustomConverter = new NullToEmptyStringSerializerConverter();
        }
    }

<<<<<<< HEAD
=======
>>>>>>> to c# 10 namespaces
=======
>>>>>>> System text json (#134)
    public static T Parse<T>(string data)
    { 
        if (string.IsNullOrEmpty(data))
            throw new ModinfoParseException("No input data.");

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> System text json (#134)
        var schema = JsonSchema.FromText(ModinfoJsonSchema.Schema);
        var validationErrors = schema.Evaluate(data, new EvaluationOptions
        {
            EvaluateAs = SpecVersion.Draft202012
        });

        if (validationErrors.HasErrors)
            throw new ModinfoParseException($"Unable to parse. Error: {validationErrors.Errors!.First()}");
<<<<<<< HEAD

        try
        {
            var parseResult = JsonSerializer.Deserialize<T>(data, SerializerOptions);
=======
        var schema = JsonSchema.FromSampleJson(ModinfoJsonSchema.Schema);
        var validationErrors = schema.Validate(data);
        if (validationErrors.Any())
            throw new ModinfoParseException($"Unable to parse. Error:{validationErrors.First()}");

        try
        {
            var parseResult = JsonConvert.DeserializeObject<T>(data);
>>>>>>> to c# 10 namespaces
=======

        try
        {
            var parseResult = JsonSerializer.Deserialize<T>(data, SerializerOptions);
>>>>>>> System text json (#134)
            if (parseResult is null)
                throw new ModinfoParseException(
                    $"Unable to parse input '{data}' to {typeof(T).Name}. Unknown Error!");
            return parseResult;
        }
<<<<<<< HEAD
<<<<<<< HEAD
        catch (JsonException cause)
=======
        catch (JsonSerializationException cause)
>>>>>>> to c# 10 namespaces
=======
        catch (JsonException cause)
>>>>>>> System text json (#134)
        {
            throw new ModinfoParseException(cause.Message, cause);
        }
    }
}