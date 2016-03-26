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

        public string ResourceType { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

        public Uri Location { get; set; }

        public string Version { get; set; }
    }
}