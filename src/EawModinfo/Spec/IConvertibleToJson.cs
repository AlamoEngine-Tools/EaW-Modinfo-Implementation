namespace EawModinfo.Spec;

/// <summary>
/// Indicates that the object can be converted to JSON data
/// </summary>
public interface IConvertibleToJson
{
    /// <summary>
    /// Converts the current object to a JSON string.
    /// </summary>
    /// <returns>The JSON data</returns>
    string ToJson();
}