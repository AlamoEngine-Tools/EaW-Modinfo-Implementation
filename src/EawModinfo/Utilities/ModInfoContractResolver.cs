﻿using System.Collections;
using System.Linq;
using System.Reflection;
using EawModinfo.Model.Json;
using EawModinfo.Spec;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EawModinfo.Utilities
{
    internal class ModInfoContractResolver : DefaultContractResolver
    {
        public static readonly ModInfoContractResolver Instance = new();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            EvaluateShouldSerialize(property);
            return property;
        }

        private static void EvaluateShouldSerialize(JsonProperty? property)
        {
            if (property is null)
                return;


            if (property.DeclaringType == typeof(JsonModinfoData) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var modinfo = (IModinfo) instance;
                        var name = property.PropertyName;
                        return name switch
                        {
                            "custom" => modinfo.Custom.Any(),
                            "languages" => modinfo.Languages.Any(),
                            "dependencies" => modinfo.Dependencies.Any(),
                            _ => true
                        };
                    };
            }
        }
    }
}