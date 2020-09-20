using Newtonsoft.Json;

namespace EawModinfo.Utilities
{
    internal static class ParseUtility
    {
        public static T Parse<T>(string data)
        { 
            if (string.IsNullOrEmpty(data))
                throw new ModinfoParseException("No input data.");
            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (JsonSerializationException cause)
            {
                throw new ModinfoParseException(cause.Message, cause);
            }
        }
    }
}
