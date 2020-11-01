using System.Collections;
using System.Linq;
using System.Reflection;
using EawModinfo.Model;
using EawModinfo.Spec;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EawModinfo.Utilities
{
    internal class ModInfoContractResolver : DefaultContractResolver
    {
        public static readonly ModInfoContractResolver Instance = new ModInfoContractResolver();

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


            if (property.DeclaringType == typeof(ModinfoData) && typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize =
                    instance =>
                    {
                        var modinfo = (IModinfo) instance;
                        var name = property.PropertyName;
                        switch (name)
                        {
                            case "custom":
                                return modinfo.Custom.Any();
                            case "languages":
                                return modinfo.Languages.Any();
                            case "dependencies":
                                return modinfo.Dependencies.Any();
                            default:
                                return true;
                        }
                    };
            }
        }
    }
}