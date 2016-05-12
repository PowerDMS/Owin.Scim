namespace Owin.Scim.Services
{
    using Configuration;

    using Model;

    public abstract class ServiceBase
    {
        protected ServiceBase(ScimServerConfiguration serverConfiguration, IResourceVersionProvider versionProvider)
        {
            ServerConfiguration = serverConfiguration;
            VersionProvider = versionProvider;
        }

        public IResourceVersionProvider VersionProvider { get; private set; }

        public ScimServerConfiguration ServerConfiguration { get; private set; }

        protected virtual TResource SetResourceVersion<TResource>(TResource resource)
            where TResource : Resource
        {
            if (resource == null) return null;

            var etagConfig = ServerConfiguration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag);

            if (!etagConfig.Supported) return resource;

            // Only calculate the version hash if it's empty.
            // If it's not null, then the version may be coming from the repository implementation.
            // This allows implementors to provide strong and/or persisted version hashes.
            if (resource.Meta != null && string.IsNullOrWhiteSpace(resource.Meta.Version))
            {
                // Below, we leak service-layer etag logic into the business logic on purpose.
                // SCIM's Meta.Version property MUST mirror the ETag header.
                var etagPrefix = etagConfig.IsWeak ? @"W/" : string.Empty;
                resource.Meta.Version = string.Format("{0}\"{1}\"", etagPrefix, VersionProvider.GenerateVersion(resource)); 
            }

            return resource;
        }
    }
}