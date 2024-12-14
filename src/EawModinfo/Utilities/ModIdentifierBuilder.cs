using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Text;
using EawModinfo.Model;
using EawModinfo.Spec;

namespace EawModinfo.Utilities;

/// <summary>
/// Provides the methods to generate mod identifiers. 
/// </summary>
public static class ModReferenceBuilder
{
    /// <summary>
    /// Specifies the kind of the installation location of a mod.
    /// </summary>
    public enum ModLocationKind
    {
        /// <summary>
        /// The mod is installed in the game's GAME/Mods/ directory.
        /// </summary>
        GameModsDirectory,
        /// <summary>
        /// The mod is installed in correct Steam Workshops directory.
        /// </summary>
        SteamWorkshops,
        /// <summary>
        /// The mod is installed on any other location than <see cref="GameModsDirectory"/> and <see cref="SteamWorkshops"/>.
        /// </summary>
        External
    }

    /// <summary>
    /// Creates a mod reference for a virtual mod from the specified modinfo data.
    /// </summary>
    /// <param name="modinfo">The modinfo data to create a virtual mod reference from.</param>
    /// <returns>A mod reference for a virtual mod.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modinfo"/> is <see langword="null"/>.</exception>
    public static IModReference CreateVirtualModIdentifier(ModinfoData modinfo)
    {
        if (modinfo == null) 
            throw new ArgumentNullException(nameof(modinfo));
        return new ModReference(modinfo.ToJson(), ModType.Virtual);
    }

    /// <summary>
    /// Creates an enumerable collection of mod references from the specified <see cref="ModinfoFinderCollection"/>.
    /// </summary>
    /// <remarks>
    /// The mod references and their respective identifiers are created based on sections I.4 and III.2.4 from the modinfo specification.
    /// <br/>
    /// This method does not sanity check the collection's location against <paramref name="locationKind"/>,
    /// except that <see cref="ModLocationKind.SteamWorkshops"/> requires the directory name to be convertible into a Steam Workshops ID. 
    /// </remarks>
    /// <param name="modinfoFinderResult">The collection that contains the found modinfo files for a directory.</param>
    /// <param name="locationKind">Specifies the location kind </param>
    /// <returns>An enumerable collection of mod references.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="modinfoFinderResult"/> is <see langword="null"/>.</exception>
    /// <exception cref="ModinfoException"><paramref name="locationKind"/> is <see langword="true"/> but the directory name of <paramref name="modinfoFinderResult"/> is not a valid Steam Workshops ID.</exception>
    public static IEnumerable<DetectedModReference> CreateIdentifiers(ModinfoFinderCollection modinfoFinderResult, ModLocationKind locationKind)
    {
        if (modinfoFinderResult == null) 
            throw new ArgumentNullException(nameof(modinfoFinderResult));

        if (locationKind == ModLocationKind.SteamWorkshops) 
            ModinfoValidator.ValidateSteamWorkshopsId(modinfoFinderResult.Directory.Name);

        var modDir = modinfoFinderResult.Directory;

        // Using 1 as initial capacity as that's probably the case for 99% of existing mod installations.
        var result = new List<DetectedModReference>(1);

        if (modinfoFinderResult.HasMainModinfoFile)
        {
            // Rules ii), iv) and v) from I.4.2 apply here
            modinfoFinderResult.MainModinfo.TryGetModinfo(out var mainModinfo);
            result.Add(CreateDetectedModReference(modDir, locationKind, mainModinfo, false));
        }

        if (modinfoFinderResult.HasVariantModinfoFiles)
        {
            foreach (var variant in modinfoFinderResult.Variants)
            {
                if (!variant.TryGetModinfo(out var variantData))
                    continue;

                var variantReference = CreateDetectedModReference(modDir, locationKind, variantData, true);
                result.Add(variantReference);
            }
        }

        // This ensures rules i) and vi) from I.4.2
        if (result.Count == 0) 
            result.Add(CreateDetectedModReference(modDir, locationKind, null, false));

        return result;
    }


    private static DetectedModReference CreateDetectedModReference(
        IDirectoryInfo modDir,
        ModLocationKind locationKind,
        IModinfo? modinfo,
        bool appendName)
    {
        var modType = locationKind == ModLocationKind.SteamWorkshops ? ModType.Workshops : ModType.Default;

        var sb = new StringBuilder();
        sb.Append(locationKind is ModLocationKind.GameModsDirectory or ModLocationKind.SteamWorkshops
            ? modDir.Name
            : modDir.FullName);

        if (appendName)
        {
            Debug.Assert(modinfo is not null);
            sb.Append(':');
            sb.Append(modinfo!.Name);
        }

        var modReference = new ModReference(sb.ToString(), modType);
        return new DetectedModReference(modReference, modDir, modinfo);
    }
}