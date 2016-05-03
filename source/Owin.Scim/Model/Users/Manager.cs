namespace Owin.Scim.Model.Users
{
    using System;

    using Newtonsoft.Json;
    
    public class Manager
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("$ref")]
        public Uri Ref { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}