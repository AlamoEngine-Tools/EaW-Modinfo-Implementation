using System.Linq;
using Newtonsoft.Json;
using NJsonSchema;

namespace EawModinfo.Utilities
{
    internal static class ParseUtility
    {
        public static T Parse<T>(string data)
        { 
            if (string.IsNullOrEmpty(data))
                throw new ModinfoParseException("No input data.");

            var schema = JsonSchema.FromSampleJson(ModinfoJsonSchema.Schema);
            var validationErrors = schema.Validate(data);
            if (validationErrors.Any())
                throw new ModinfoParseException($"Unable to parse. Error:{validationErrors.First()}");

            try
            {
                var parseResult = JsonConvert.DeserializeObject<T>(data);
                if (parseResult is null)
                    throw new ModinfoParseException(
                        $"Unable to parse input '{data}' to {typeof(T).Name}. Unknown Error!");
                return parseResult;
            }
            catch (JsonSerializationException cause)
            {
                throw new ModinfoParseException(cause.Message, cause);
            }
        }
    }
}
