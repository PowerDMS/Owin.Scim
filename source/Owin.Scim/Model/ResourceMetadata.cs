namespace Owin.Scim.Model
{
    using System;

    using Newtonsoft.Json;
    
    public sealed class ResourceMetadata
    {
        public ResourceMetadata(string resourceType)
        {
            ResourceType = resourceType;
        }

        [JsonConstructor]
        private ResourceMetadata()
        {
        }

        [JsonProperty("resourceType")]
        public string ResourceType { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        [JsonProperty("location")]
        public Uri Location { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}