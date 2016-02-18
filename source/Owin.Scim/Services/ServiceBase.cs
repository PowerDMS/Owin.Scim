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

        public IResourceVersionProvider VersionProvider { get; set; }

        protected virtual TResource SetResourceVersion<TResource>(TResource resource)
            where TResource : Resource
        {
            if (resource == null) return null;

            var etagConfig = _Configuration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag);

            if (!etagConfig.Supported) return resource;

            // Only calculate the version hash if it's empty.
            // If it's not null, then the version may be coming from the repository impl.
            // This allows implementors to provide strong and/or persisted version hashes.
            if (resource.Meta != null && string.IsNullOrWhiteSpace(resource.Meta.Version))
            {
                // Below, we leak service-layer etag logic into the business logic on purpose.
                // SCIM's Meta.Version property MUST mirror the ETag header.
                var etagPrefix = etagConfig.IsWeak ? @"W/" : string.Empty;
                resource.Meta.Version = $"{etagPrefix}\"{VersionProvider.GenerateVersion(resource)}\""; 
            }

            return resource;
        }
    }
}