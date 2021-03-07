using System.Reflection;
using EawModinfo.Model.Steam;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EawModinfo.Utilities
{
    internal class SteamDataResolver : DefaultContractResolver
    {
        public static SteamDataResolver Instance = new();
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(SteamData) && property.PropertyType == typeof(string))
            {
                switch (member.Name)
                {
                    case nameof(SteamData.Metadata):
                    case nameof(SteamData.PreviewFile):
                    case nameof(SteamData.Description):
                        property.ValueProvider = new NullToEmptyStringValueProvider(property.ValueProvider);
                        break;
                }
            }

            return property;
        }

        private class NullToEmptyStringValueProvider : IValueProvider
        {
            private readonly IValueProvider? _provider;
            public NullToEmptyStringValueProvider(IValueProvider? provider)
            {
                _provider = provider;
            }

            public object GetValue(object target)
            {
                return _provider?.GetValue(target) ?? string.Empty;
            }

            public void SetValue(object target, object? value)
            {
                _provider?.SetValue(target, value);
            }
        }
    }
}