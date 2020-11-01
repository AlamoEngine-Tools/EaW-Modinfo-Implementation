using System.Collections.Generic;

namespace EawModinfo.Spec.Steam
{
    /// <summary>
    /// Data structure which represents important properties from the workshops uploader.
    /// </summary>
    public interface ISteamData
    {
        /// <summary>
        /// The Steam Workshop ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The content folder's name as specified by the Steam Uploader.
        /// </summary>
        public string ContentFolder { get; }

        /// <summary>
        /// The visibility of the mod.
        /// </summary>
        public SteamWorkshopVisibility Visibility { get; }
        
        /// <summary>
        /// Arbitrary metadata as string. Can be <see langword="null"/>
        /// </summary>
        public string? Metadata { get; }

        /// <summary>
        /// Steam Tags as specified by the Steam Uploader.
        /// </summary>
        public IEnumerable<string> Tags { get; }

        /// <summary>
        /// Description of the mod in Steam flavoured BB-Code.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Relative path to an image file which holds the preview image
        /// </summary>
        public string? PreviewFile { get; }

        /// <summary>
        /// The display name of the mod in Steam Workshops.
        /// </summary>
        public string Title { get; }
    }
}