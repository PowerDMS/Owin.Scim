namespace Owin.Scim.Services
{
    using Configuration;

    using Model;

    public class ServiceBase
    {
        private readonly ScimServerConfiguration _Configuration;

        public ServiceBase(ScimServerConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public IResourceETagProvider ETagProvider { get; set; }

        protected virtual TResource CalculateVersion<TResource>(TResource resource)
            where TResource : Resource
        {
            if (resource == null) return null;

            var etagConfig = _Configuration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag);

            if (!etagConfig.Supported) return resource;

            // Only calculate the etag hash if it's empty.
            // If it's not null, then the etag is coming from the repository impl.
            // This allows implementors to provide strong etag hashes.
            if (resource.Meta != null && string.IsNullOrWhiteSpace(resource.Meta.Version))
            {
                var etagPrefix = etagConfig.IsWeak ? @"W/" : string.Empty;
                resource.Meta.Version = $"{etagPrefix}\"{ETagProvider.GenerateETag(resource)}\""; 
            }

            return resource;
        }
    }
}