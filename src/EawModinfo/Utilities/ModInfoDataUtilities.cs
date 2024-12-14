using System;
using System.Collections.Generic;
using System.Linq;
using EawModinfo.Model;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

/// <summary>
/// Provides various utilities for an <see cref="IModinfo"/>.
/// </summary>
public static class ModinfoDataUtilities
{
    /// <summary>
    /// Validates both inputs then creates a copy of
    /// <paramref name="baseModinfo"/> and overrides existing data from <paramref name="target"/> into the new copy.
    /// If <paramref name="baseModinfo"/> is <see langword="null"/> this method will return <paramref name="target"/>.
    /// </summary>
    /// <remarks>The values of <see cref="IModinfo.Languages"/>, <see cref="IModIdentity.Dependencies"/> and <see cref="IModinfo.SteamData"/> get replaced entirely.
    /// <see cref="IModinfo.Custom"/> will be merged by key/value pair where values of the same key
    /// from <paramref name="target"/> are superior to <paramref name="baseModinfo"/>.
    /// </remarks>
    /// <param name="target">The data source into which data will get merged.</param>
    /// <param name="baseModinfo">Base data which will get merged to <paramref name="baseModinfo"/>.</param>
    /// <returns>A merged modinfo or <paramref name="target"/> if <paramref name="baseModinfo"/> was <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
    public static IModinfo MergeInto(this IModinfo target, IModinfo? baseModinfo)
    {
        if (target is null)
            throw new ArgumentNullException(nameof(target));
        if (baseModinfo is null)
            return target;
        target.Validate();
        baseModinfo.Validate();
        return MergeFrom(baseModinfo, target);
    }
        
    private static ModinfoData MergeFrom(IModinfo current, IModinfo target)
    { 
        var name = target.Name;

        var summary = current.Summary;
        if (!string.IsNullOrEmpty(target.Summary))
            summary = target.Summary;

        var icon = current.Icon;
        if (!string.IsNullOrEmpty(target.Icon))
            icon = target.Icon;

        var version = current.Version;
        if (target.Version != null)
            version = target.Version;

        var custom = new Dictionary<string, object>(target.Custom);
        if (current.Custom.Any())
        {
            foreach (var customObject in current.Custom)
            {
                if (custom.ContainsKey(customObject.Key))
                    continue;
                custom.Add(customObject.Key, customObject.Value);
            }
        }

        var steamData = current.SteamData;
        if (target.SteamData != null)
            steamData = new SteamData(target.SteamData);

        var dependencies = current.Dependencies;
        if (target.Dependencies.Any())
            dependencies = new DependencyList(target.Dependencies);

        var languages = current.Languages;
        if (target.LanguagesExplicitlySet) 
            languages = target.Languages.Select(x => (ILanguageInfo)new LanguageInfo(x)).Distinct().ToList();

        return new ModinfoData(name)
        {
            Custom = custom,
            SteamData = steamData,
            Summary = summary,
            Dependencies = dependencies,
            Icon = icon,
            Languages = languages,
            Version = version
        };
    }
}