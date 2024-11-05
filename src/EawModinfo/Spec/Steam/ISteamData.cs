using System.Collections.Generic;

namespace EawModinfo.Spec.Steam;

/// <summary>
/// Represents the data structure used by the Steam workshops uploader for submitting mods and updates.
/// </summary>
public interface ISteamData : IConvertibleToJson
{
    /// <summary>
    /// Gets the Steam Workshop ID.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the content folder's name as specified by the Steam Uploader.
    /// </summary>
    public string ContentFolder { get; }

    /// <summary>
    /// Gets the visibility of the mod.
    /// </summary>
    public SteamWorkshopVisibility Visibility { get; }
        
    /// <summary>
    /// Gets arbitrary metadata as string. Can be <see langword="null"/>
    /// </summary>
    public string? Metadata { get; }

    /// <summary>
    /// Steam Tags as specified by the Steam Workshop documentation.
    /// </summary>
    /// <remarks>
    /// Tags must be unique, are case-sensitive, are limited to 255 characters and must not contain the comma ',' character.
    /// </remarks>
    public IEnumerable<string> Tags { get; }

    /// <summary>
    /// Gets the description of the mod in Steam flavoured BB-Code.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Gets the path to an image file which holds the preview image used by the Steam uploader
    /// </summary>
    public string? PreviewFile { get; }

    /// <summary>
    /// Gets the display name of the mod in Steam Workshops.
    /// </summary>
    public string Title { get; }
}