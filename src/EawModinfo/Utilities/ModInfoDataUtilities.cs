using System;
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
    /// <remarks>Subsequent data such as <see cref="IModinfo.Languages"/> will be replaced entirely and not merged by property.
    /// Exception is <see cref="IModinfo.Custom"/>, where items will get merged individually.
    /// <br></br>
    /// Subsequent data get replaced by creating a new copy of that element. This means the new and the merged property are not equal by reference.
    /// </remarks>
    /// <param name="target">the data source from which data will get merged.</param>
    /// <param name="baseModinfo">Original data which will get updated.</param>
    /// <returns>A new instance of an <see cref="IModinfo"/> or <paramref name="target"/> if <paramref name="baseModinfo"/> was <see langword="null"/></returns>
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

        var custom = current.Custom;
        if (target.Custom.Any())
        {
            foreach (var customObject in target.Custom)
            {
                if (custom.Contains(customObject))
                    continue;
                custom.Add(customObject);
            }
        }

        var steamData = current.SteamData;
        if (target.SteamData != null)
            steamData = new SteamData(target.SteamData);

        var dependencies = current.Dependencies;
        if (target.Dependencies.Any())
            dependencies = new DependencyList(target.Dependencies);

        var languages = current.Languages;
        if (target.Languages.Any())
        {
#if NETSTANDARD2_1
                    languages = target.Languages.Select(x => (ILanguageInfo) new LanguageInfo(x)).ToHashSet(null);
#else
            languages = target.Languages.Select(x => (ILanguageInfo)new LanguageInfo(x)).Distinct();
#endif
        }

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