using System;
using System.IO;

namespace EawModinfo.Spec;

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

    /// <summary>
    /// Converts the current instance to JSON and writes it to the stream.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <exception cref="ModinfoException">The instance to serialize is not valid again the modinfo specification.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    void ToJson(Stream stream);
}