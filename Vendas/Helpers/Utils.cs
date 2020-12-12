using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Vendas.Helpers
{
    public static class Utils
    {
        private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(false);

        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None,
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };

        public static byte[] ToJsonBytes(this object source)
        {
            if (source == null)
                return null;
            var instring = JsonConvert.SerializeObject(source, Formatting.Indented, JsonSettings);
            return Utf8NoBom.GetBytes(instring);
        }

        /// <summary>
        /// Parses a Utf8 byte json to a specific object.
        /// </summary>
        /// <typeparam name="T">type of object to be parsed.</typeparam>
        /// <param name="json">The json bytes.</param>
        /// <returns>the object parsed from json.</returns>
        public static T ParseJson<T>(this byte[] json)
        {
            if (json == null || json.Length == 0) return default;
            var result = JsonConvert.DeserializeObject<T>(Utf8NoBom.GetString(json), JsonSettings);
            return result;
        }
    }
}
