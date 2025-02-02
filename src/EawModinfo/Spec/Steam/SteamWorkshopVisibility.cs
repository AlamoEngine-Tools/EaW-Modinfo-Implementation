namespace AET.Modinfo.Spec.Steam;

/// <summary>
/// Possible visibility states that a Workshop item can be in.
/// </summary>
public enum SteamWorkshopVisibility
{
    /// <summary>
    /// Visible to everyone.
    /// </summary>
    Public = 0,
    /// <summary>
    /// Visible to friends only.
    /// </summary>
    FriendsOnly = 1,
    /// <summary>
    /// Only visible to the creator.
    /// </summary>
    Private = 2,
    /// <summary>
    /// Visible to everyone, but will not be returned in any global queries.
    /// </summary>
    Unlisted = 3
}