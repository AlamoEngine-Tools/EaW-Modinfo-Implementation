﻿using System.Globalization;
using EawModinfo.Spec;
using EawModinfo.Utilities;
using Newtonsoft.Json;
using Validation;

#if NETSTANDARD2_1 || NET
using System;
#endif

namespace EawModinfo.Model
{
    /// <inheritdoc/>
    public class LanguageInfo : ILanguageInfo
    {
        /// <summary>
        /// Returns an <see cref="ILanguageInfo"/> which represents ENGLISH (en) where <see cref="ILanguageInfo.Support"/> is set to <see cref="LanguageSupportLevel.Default"/>
        /// </summary>
        public static readonly ILanguageInfo Default = new LanguageInfo {Code = "en", Support = LanguageSupportLevel.FullLocalized};

        [JsonIgnore] private CultureInfo? _culture;

        /// <inheritdoc/>
        [JsonProperty("code")] public string Code { get; internal set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("support", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public LanguageSupportLevel Support { get; internal set; }

        /// <summary>
        /// Gets a culture representation of the <see cref="Code"/> property.
        /// </summary>
        [JsonIgnore]
        public CultureInfo Culture => _culture ??= new CultureInfo(Code);

        [JsonConstructor]
        internal LanguageInfo()
        {
        }

        /// <summary>
        /// Creates a new instance from a given <see cref="ILanguageInfo"/> instance.
        /// </summary>
        /// <param name="languageInfo">The instance that will copied.</param>
        public LanguageInfo(ILanguageInfo languageInfo)
        {
            Requires.NotNull(languageInfo, nameof(languageInfo));
            Code = languageInfo.Code;
            Support = languageInfo.Support;
        }
        
        /// <summary>
        /// Parses and deserializes a json data into a <see cref="LanguageInfo"/>
        /// </summary>
        /// <param name="data">The raw json data.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ModinfoParseException">Throws when parsing failed due to missing required properties.</exception>
        public static LanguageInfo Parse(string data)
        {
            return ParseUtility.Parse<LanguageInfo>(data);
        }

        /// <inheritdoc/>
        public bool Equals(ILanguageInfo? other)
        {
            if (other is null) 
                return false;
            if (ReferenceEquals(this, other)) return true;
            return Code == other.Code;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is null) 
                return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is ILanguageInfo info) return Equals(info);
            return false;

        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Culture.GetHashCode();
        }
    }
}