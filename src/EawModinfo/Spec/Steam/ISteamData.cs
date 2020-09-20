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
    }
}