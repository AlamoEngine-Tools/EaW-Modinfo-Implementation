namespace EawModinfo.Spec;

// TODO: Support streams

/// <summary>
/// Indicates that an instance of the interface can be converted to JSON data
/// </summary>
public interface IConvertibleToJson
{
    /// <summary>
    /// Converts the current instance to a JSON string.
    /// </summary>
    /// <returns>The serialized JSON data.</returns>
    /// <exception cref="ModinfoException">The instance to serialize is not valid again the modinfo specification.</exception>
    string ToJson();
}