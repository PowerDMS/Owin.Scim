namespace Owin.Scim.Model
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public abstract class Resource
    {
        [JsonProperty(Order = -5)]
        public string Id { get; set; }

        public string ExternalId { get; set; }

        [JsonProperty(Order = -10)]
        public abstract IEnumerable<string> Schemas { get; }

        public ResourceMetadata Meta { get; set; }
    }
}