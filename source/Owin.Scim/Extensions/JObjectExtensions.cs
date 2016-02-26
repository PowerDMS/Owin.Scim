namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    public static class JObjectExtensions
    {
        public static string FindKeyCaseInsensitive(this JObject jObject, string key)
        {
            foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
            {
                if (keyValuePair.Key.Equals(key, StringComparison.OrdinalIgnoreCase)) return keyValuePair.Key;
            }

            return null;
        }
    }
}