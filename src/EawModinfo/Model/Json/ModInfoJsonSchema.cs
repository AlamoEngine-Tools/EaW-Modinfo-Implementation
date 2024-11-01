using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Json.Schema;

namespace EawModinfo.Model.Json;

/// <summary>
/// 
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
            OutputFormat = OutputFormat.List,
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

    /// <summary>
    /// 
    /// </summary>
    public const string ModRefSchema = @"{
  ""$id"": ""https://AlamoEngine-Tools.github.com/mod-ref"",
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

    /// <summary>
    /// 
    /// </summary>
    public const string DependenciesSchema = @"{
  ""$id"": ""https://AlamoEngine-Tools.github.com/mod-deps"",
  ""$schema"": ""https://json-schema.org/draft/2020-12/schema"",
  ""description"": ""Represents a mod's dependencies as an ordered array."",
  ""type"": ""array"",
  ""contains"": {
    ""type"":""object"",
    ""$ref"": ""/mod-ref""
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
        ""$ref"": ""/mod-ref""
      }
    },
    {
      ""items"": {
        ""type"":""object"",
        ""$ref"": ""/mod-ref""
      }
    }
  ]
}";

    /// <summary>
    /// 
    /// </summary>
    public const string LanguageInfoSchema = @"{
  ""$id"":""https://AlamoEngine-Tools.github.com/lang-info"",
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

    /// <summary>
    /// 
    /// </summary>
    public const string SteamDataSchema = @"{
  ""$id"": ""https://AlamoEngine-Tools.github.com/steam-data"",
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

    /// <summary>
    /// 
    /// </summary>
    public const string ModInfo = @"{
  ""$id"":""https://AlamoEngine-Tools.github.com/modinfo"",
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
	  ""$ref"":""/mod-deps""
    },
    ""languages"":{
      ""type"":""array"",
      ""items"":{
        ""$ref"":""/lang-info""
      }
    },
    ""steamdata"":{
      ""type"":""object"",
      ""$ref"":""/steam-data""
    }
  },
  ""required"":[
    ""name""
  ]
}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    /// <param name="evaluationType"></param>
    public static void Evaluate(JsonNode? json, EvaluationType evaluationType)
    {
        var schema = GetSchemaForType(evaluationType);
        var validationErrors = schema.Evaluate(json, EvaluationOptions);
        ThrowOnValidationError(validationErrors);
    }

    internal static void Evaluate<T>(JsonNode? json)
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
                new Uri("https://AlamoEngine-Tools.github.com/mod-ref", UriKind.Absolute)),
            EvaluationType.SteamData => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.com/steam-data", UriKind.Absolute)),
            EvaluationType.ModLanguageInfo => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.com/lang-info", UriKind.Absolute)),
            EvaluationType.ModDependencyList => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.com/mod-deps", UriKind.Absolute)),
            EvaluationType.ModInfo => (JsonSchema)Registry.Get(
                new Uri("https://AlamoEngine-Tools.github.com/modinfo", UriKind.Absolute)),
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

/// <summary>
/// 
/// </summary>
public enum EvaluationType
{
    /// <summary>
    /// 
    /// </summary>
    ModInfo,
    /// <summary>
    /// 
    /// </summary>
    ModReference,
    /// <summary>
    /// 
    /// </summary>
    ModDependencyList,
    /// <summary>
    /// 
    /// </summary>
    ModLanguageInfo,
    /// <summary>
    /// 
    /// </summary>
    SteamData
}