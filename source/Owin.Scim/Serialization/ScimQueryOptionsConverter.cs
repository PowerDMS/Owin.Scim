namespace Owin.Scim.Serialization
{
    using System;
    using System.Collections.Generic;

    using Configuration;

    using Extensions;

    using Microsoft.Owin;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Querying;

    public class ScimQueryOptionsConverter : JsonConverter
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        public ScimQueryOptionsConverter(ScimServerConfiguration serverConfiguration)
        {
            _ServerConfiguration = serverConfiguration;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(ScimQueryOptions).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var keyValues = new Dictionary<string, string[]>();
            foreach (var kvp in (IDictionary<string, JToken>) JObject.Load(reader))
            {
                var values = new List<string>();
                if (kvp.Value is JValue)
                    values.Add(kvp.Value.ToString());
                else if (kvp.Value is JArray)
                    values.AddRange(kvp.Value.Values<string>());
                else
                    continue;

                keyValues.Add(kvp.Key, values.ToArray());
            }

            return new ReadableStringCollection(keyValues).GetScimQueryOptions(_ServerConfiguration);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}