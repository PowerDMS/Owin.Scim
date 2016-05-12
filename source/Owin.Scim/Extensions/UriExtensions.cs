namespace Owin.Scim.Extensions
{
    using System;
    using System.Linq;

    using Configuration;

    using Model;

    using Services;

    public static class UriExtensions
    {
        /// <summary>
        /// Determines whether the specified URI is a reference to the local SCIM server.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns><c>true</c> if the specified URI is a reference to the local SCIM server; otherwise, <c>false</c>.</returns>
        public static bool IsScimServerUri(this Uri uri)
        {
            return uri.ToString().StartsWith(AmbientRequestService.BaseUri.AbsoluteUri);
        }

        /// <summary>
        /// Parses the <paramref name="uri"/> as a reference to a local resource. 
        /// Returns null if the specified <paramref name="uri"/> is invalid.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="serverConfiguration">The server configuration.</param>
        /// <returns>ScimResourceReference.</returns>
        public static ScimResourceReference ToScimResourceReference(this Uri uri, ScimServerConfiguration serverConfiguration)
        {
            var scimPathBase = AmbientRequestService.BasePath;
            var uriPaths = (string.IsNullOrWhiteSpace(scimPathBase)
                ? uri.AbsolutePath
                : uri.AbsolutePath.Replace(scimPathBase, ""))
                .TrimStart('/')
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (uriPaths.Length < 2)
                return null;

            var resourceEndpoint = uriPaths[0];
            var resourceId = uriPaths[1];
            
            var resourceDefinition = serverConfiguration
                .ResourceTypeDefinitions
                .SingleOrDefault(rtd => rtd.Endpoint.IndexOf(resourceEndpoint, StringComparison.OrdinalIgnoreCase) >= 0);

            if (resourceDefinition == null)
                return null; // invalid uri

            return new ScimResourceReference(resourceDefinition, resourceId);
        }
    }
}