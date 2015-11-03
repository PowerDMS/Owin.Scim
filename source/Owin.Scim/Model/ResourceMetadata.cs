namespace Owin.Scim.Model
{
    using System;

    public sealed class ResourceMetadata
    {
        public string ResourceType { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

        public Uri Location { get; set; }

        public string Version { get; set; }
    }
}