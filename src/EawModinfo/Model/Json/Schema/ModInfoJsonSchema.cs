using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using Json.Schema;

namespace AET.Modinfo.Model.Json.Schema;

/// <summary>
/// Provides operations to validate JSON data against the modinfo JSON schema specification.
/// </summary>
public static class ModInfoJsonSchema
{
    private static readonly SchemaRegistry Registry;
    private static readonly EvaluationOptions EvaluationOptions;
    private static readonly BuildOptions BuildOptions;

    static ModInfoJsonSchema()
    {
        var buildOptions = new BuildOptions
        {
            Dialect = Dialect.Draft202012
        };

        var registry = buildOptions.SchemaRegistry;

        registry.Register(GetSchemaFromResource("modref.json"));
        registry.Register(GetSchemaFromResource("moddependencies.json"));
        registry.Register(GetSchemaFromResource("languageinfo.json"));
        registry.Register(GetSchemaFromResource("steamdata.json"));
        registry.Register(GetSchemaFromResource("modinfo.json"));

        BuildOptions = buildOptions;
        Registry = registry;
        EvaluationOptions = new EvaluationOptions
        {
            OutputFormat = OutputFormat.Hierarchical,
        };
    }

    private static JsonSchema GetSchemaFromResource(string schema)
    {
        using var resourceStream = typeof(ModInfoJsonSchema)
            .Assembly.GetManifestResourceStream($"AET.Modinfo.Resources.Schemas._3._0._0.{schema}");
        Debug.Assert(resourceStream is not null);
        var json = JsonDocument.Parse(resourceStream!).RootElement;
        return JsonSchema.Build(json, BuildOptions);
    }

    /// <summary>
    /// Evaluates a JSON node against the specified modinfo JSON schema.
    /// </summary>
    /// <param name="json">The JSON node to evaluate.</param>
    /// <param name="evaluationType">The schema to use.</param>
    /// <param name="errors">When this method returns, contains the identified errors, or <see langword="null"/> if <paramref name="json"/> is valid.</param>
    /// <returns><see langword="true"/> if <paramref name="json"/> is valid against the schema; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(JsonElement json, EvaluationType evaluationType, [NotNullWhen(false)] out IReadOnlyCollection<KeyValuePair<string, string>>? errors)
    {
        errors = null;
        var schema = GetSchemaForType(evaluationType);
        var result = schema.Evaluate(json, EvaluationOptions);
        if (!result.IsValid)
        {
            var errorList = new List<KeyValuePair<string, string>>();
            CollectErrors(result, errorList);
            errors = errorList;

        }
        return result.IsValid;
    }

    private static void CollectErrors(EvaluationResults result, List<KeyValuePair<string, string>> errors)
    {
        if (result.IsValid)
            return;
        if (result.Errors is not null) 
            errors.AddRange(result.Errors!);

        if (result.Details is not null)
        {
            foreach (var detail in result.Details)
                CollectErrors(detail, errors);
        }
    }

    /// <summary>
    /// Evaluates a JSON node against the specified modinfo JSON schema.
    /// </summary>
    /// <param name="json">The JSON node to evaluate.</param>
    /// <param name="evaluationType">The schema to use.</param>
    /// <exception cref="ModinfoParseException"><paramref name="json"/> is not valid against the JSON schema specified by <paramref name="evaluationType"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
    public static void Evaluate(JsonElement json, EvaluationType evaluationType)
    {
        var schema = GetSchemaForType(evaluationType);
        var result = schema.Evaluate(json, EvaluationOptions);
        ThrowOnValidationError(result);
    }

    internal static void Evaluate<T>(JsonElement json)
    {
        Evaluate(json, GetSchemaForType(typeof(T)));
    }


    private static void ThrowOnValidationError(EvaluationResults result)
    {
        if (!result.IsValid)
        {
            var error = GetFirstError(result);
            var errorMessage = "JSON not valid";

            if (error is null)
                errorMessage += ": Unknown Error";
            else
                errorMessage += $": {error}";

            throw new ModinfoParseException(errorMessage);
        }
    }

    private static KeyValuePair<string, string>? GetFirstError(EvaluationResults result)
    {
        if (result.Errors is not null)
            return result.Errors!.First();
        if (result.Details is not null)
        {
            foreach (var child in result.Details!)
            {
                var error = GetFirstError(child);
                if (error is not null)
                    return error;
            }
        }
        return null;
    }

    private static JsonSchema GetSchemaForType(EvaluationType type)
    {
        return type switch
        {
            EvaluationType.ModReference => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/mod-ref", UriKind.Absolute))!,
            EvaluationType.SteamData => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/steam-data", UriKind.Absolute))!,
            EvaluationType.ModLanguageInfo => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/lang-info", UriKind.Absolute))!,
            EvaluationType.ModDependencyList => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/mod-deps", UriKind.Absolute))!,
            EvaluationType.ModInfo => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/modinfo", UriKind.Absolute))!,
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }

    private static EvaluationType GetSchemaForType(Type type)
    {
        if (type == typeof(JsonModReference))
            return EvaluationType.ModReference;
        if (type == typeof(JsonSteamData))
            return EvaluationType.SteamData;
        if (type == typeof(JsonLanguageInfo))
            return EvaluationType.ModLanguageInfo;
        if (type == typeof(JsonDependencyList))
            return EvaluationType.ModDependencyList;
        if (type == typeof(JsonModinfoData))
            return EvaluationType.ModInfo;
        throw new ArgumentOutOfRangeException(nameof(type), $"Unable to get EvaluationType for type '{type.FullName}'");
    }
}