using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Nodes;
using Json.Schema;

namespace EawModinfo.Model.Json.Schema;

/// <summary>
/// Performs operations to validate JSON data against the modinfo JSON schema specification.
/// </summary>
public static class ModInfoJsonSchema
{
    private static readonly SchemaRegistry Registry;
    private static readonly EvaluationOptions EvaluationOptions;

    static ModInfoJsonSchema()
    {
        var evalvOptions = new EvaluationOptions
        {
            EvaluateAs = SpecVersion.Draft202012,
            OutputFormat = OutputFormat.Hierarchical,
            AllowReferencesIntoUnknownKeywords = false
        };
        var registry = evalvOptions.SchemaRegistry;

        registry.Register(JsonSchema.FromText(DependenciesSchema));
        registry.Register(JsonSchema.FromText(ModRefSchema));
        registry.Register(JsonSchema.FromText(LanguageInfoSchema));
        registry.Register(JsonSchema.FromText(SteamDataSchema));
        registry.Register(JsonSchema.FromText(ModInfo));

        Registry = evalvOptions.SchemaRegistry;
        EvaluationOptions = evalvOptions;
    }

    private static JsonSchema GetSchemaFromResource(string schema)
    {
        return null!;
    }

    internal const string ModRefSchema = @"{
  ""$id"": ""https://AlamoEngine-Tools.github.io/schemas/3.0.0/mod-ref"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""description"": ""Represents a reference to a mod."",
  ""type"":""object"",
      ""required"":[
        ""modtype"",
        ""identifier""
      ],
      ""properties"":{
        ""modtype"":{
          ""type"":""number"",
          ""minimum"":0,
          ""maximum"":2
        },
        ""identifier"":{
          ""type"":""string"",
          ""minLength"": 1
        },
        ""version-range"":{
          ""type"":""string""
        }
      },
      ""additionalProperties"":false
}";

    internal const string DependenciesSchema = @"{
  ""$id"": ""https://AlamoEngine-Tools.github.io/schemas/3.0.0/mod-deps"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""description"": ""Represents a mod's dependencies as an ordered array."",
  ""type"": ""array"",
  ""contains"": {
    ""type"":""object"",
    ""$ref"": ""/schemas/3.0.0/mod-ref""
  },
  ""additionalItems"": false,
  ""oneOf"": [
    {
      ""prefixItems"": [
        {
          ""enum"": [
            ""ResolveRecursive"",
            ""ResolveLastItem"",
            ""FullResolved""
          ]
        }
      ],
      ""items"": {
        ""type"":""object"",
        ""$ref"": ""/schemas/3.0.0/mod-ref""
      }
    },
    {
      ""items"": {
        ""type"":""object"",
        ""$ref"": ""/schemas/3.0.0/mod-ref""
      }
    }
  ]
}";

    internal const string LanguageInfoSchema = @"{
  ""$id"":""https://AlamoEngine-Tools.github.io/schemas/3.0.0/lang-info"",
  ""$schema"":""https://json-schema.org/draft/2020-12/schema"",
  ""description"":""Represents the level of localizatio support for a single language."",
  ""type"":""object"",
  ""properties"":{
    ""code"":{
      ""type"":""string"",
      ""minLength"":2,
      ""maxLength"":2
    },
    ""support"":{
      ""type"":""number"",
      ""minimum"":1,
      ""maximum"":7
    }
  },

   ""additionalProperties"":false
}";

    internal const string SteamDataSchema = @"{
  ""$id"": ""https://AlamoEngine-Tools.github.io/schemas/3.0.0/steam-data"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""description"": ""Represents the steam information ."",
  ""type"": ""object"",
  ""$defs"": {
    ""tagType"":{
      ""type"":""string"",
      ""pattern"": ""^[^,\u0000-\u001F\u007F-\uFFFF]*$"",
       ""maxLength"": 255 
    }  
  },
  ""required"": [
    ""publishedfileid"",
    ""contentfolder"",
    ""visibility"",
    ""title"",
    ""tags""
  ],
  ""properties"": {
    ""publishedfileid"": {
      ""type"": ""string""
    },
    ""contentfolder"": {
      ""type"": ""string""
    },
    ""visibility"": {
      ""type"": ""integer"",
      ""minimum"": 0,
      ""maximum"": 3
    },
    ""title"": {
      ""type"": ""string""
    },
    ""metadata"": {
      ""type"": ""string""
    },
    ""tags"": {
      ""type"": ""array"",
      ""uniqueItems"": true,
      ""minItems"": 1,
      ""items"": {
        ""$ref"": ""#/$defs/tagType""
      },
      ""contains"": {
        ""oneOf"": [
          {
            ""const"": ""FOC""
          },
          {
            ""const"": ""EAW""
          }
        ]
      }
    },
    ""previewfile"": {
      ""type"": ""string""
    },
    ""description"": {
      ""type"": ""string""
    }
  },
  ""additionalProperties"": false
}";

    internal const string ModInfo = @"{
  ""$id"":""https://AlamoEngine-Tools.github.io/schemas/3.0.0/modinfo"",
  ""$schema"":""https://json-schema.org/draft/2020-12/schema"",
  ""description"":""A standard definition for Star Wars: Empire at War mod info files."",
  ""title"":""EaW Modinfo"",
  ""type"":""object"",
  ""properties"":{
    ""name"":{
      ""type"":""string"",
      ""minLength"": 1
    },
    ""summary"":{
      ""type"":""string""
    },
    ""icon"":{
      ""type"":""string""
    },
    ""version"":{
      ""type"":""string"",
      ""description"":""No validation for the version string as implementations can verify this better than JSON schemas. ""
    },
    ""dependencies"":{
      ""type"":""array"",
	  ""$ref"":""/schemas/3.0.0/mod-deps""
    },
    ""languages"":{
      ""type"":""array"",
      ""items"":{
        ""$ref"":""/schemas/3.0.0/lang-info""
      }
    },
    ""steamdata"":{
      ""type"":""object"",
      ""$ref"":""/schemas/3.0.0/steam-data""
    },
    ""custom"":{
        ""type"":""object""
    }
  },
  ""required"":[
    ""name""
  ]
}";


    // TODO: TEST this for all schemas

    /// <summary>
    /// Evaluates a JSON node against the specified modinfo JSON schema.
    /// </summary>
    /// <param name="json">The JSON node to evaluate.</param>
    /// <param name="evaluationType">The schema to use.</param>
    /// <param name="errors">When this method returns, contains the identified errors, or <see langword="null"/> if <paramref name="json"/> is valid.</param>
    /// <returns><see langword="true"/> if <paramref name="json"/> is valid against the schema; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(JsonNode? json, EvaluationType evaluationType, [NotNullWhen(false)] out IReadOnlyCollection<KeyValuePair<string, string>>? errors)
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
        if (result.HasErrors) 
            errors.AddRange(result.Errors!);

        foreach (var detail in result.Details)
            CollectErrors(detail, errors);
    }

    /// <summary>
    /// Evaluates a JSON node against the specified modinfo JSON schema.
    /// </summary>
    /// <param name="json">The JSON node to evaluate.</param>
    /// <param name="evaluationType">The schema to use.</param>
    /// <exception cref="ModinfoParseException"><paramref name="json"/> is not valid against the JSON schema specified by <paramref name="evaluationType"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
    public static void Evaluate(JsonNode json, EvaluationType evaluationType)
    {
        if (json == null) 
            throw new ArgumentNullException(nameof(json));
        var schema = GetSchemaForType(evaluationType);
        var result = schema.Evaluate(json, EvaluationOptions);
        ThrowOnValidationError(result);
    }

    internal static void Evaluate<T>(JsonNode json)
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
        if (result.HasErrors)
            return result.Errors!.First();
        foreach (var child in result.Details)
        {
            var error = GetFirstError(child);
            if (error is not null)
                return error;
        }
        return null;
    }

    private static JsonSchema GetSchemaForType(EvaluationType type)
    {
        return type switch
        {
            EvaluationType.ModReference => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/mod-ref", UriKind.Absolute)),
            EvaluationType.SteamData => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/steam-data", UriKind.Absolute)),
            EvaluationType.ModLanguageInfo => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/lang-info", UriKind.Absolute)),
            EvaluationType.ModDependencyList => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/mod-deps", UriKind.Absolute)),
            EvaluationType.ModInfo => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.io/schemas/3.0.0/modinfo", UriKind.Absolute)),
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