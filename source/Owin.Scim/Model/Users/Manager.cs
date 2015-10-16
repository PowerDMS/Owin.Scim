namespace Owin.Scim.Model.Users
{
    using System;

    using Newtonsoft.Json;

    public class Manager
    {
        public string Value { get; set; }

        [JsonProperty("$ref")]
        public Uri Ref { get; set; }

        public string DisplayName { get; set; }
    }
}