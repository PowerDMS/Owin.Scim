namespace Owin.Scim.v2.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Configuration;

    using Model;

    using Scim.Model;
    using Scim.Services;

    public class ResourceTypeService : ServiceBase, IResourceTypeService
    {
        private readonly Lazy<IReadOnlyDictionary<string, ResourceType>> _ResourceTypes;

        public ResourceTypeService(ScimServerConfiguration serverConfiguration, IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
            _ResourceTypes = new Lazy<IReadOnlyDictionary<string, ResourceType>>(CreateResourceTypes);
        }

        public virtual IReadOnlyDictionary<string, ResourceType> ResourceTypes
        {
            get { return _ResourceTypes.Value; }
        }

        public Task<IScimResponse<IEnumerable<ResourceType>>> GetResourceTypes()
        {
            return Task.FromResult<IScimResponse<IEnumerable<ResourceType>>>(
                new ScimDataResponse<IEnumerable<ResourceType>>(ResourceTypes.Values));
        }

        public Task<IScimResponse<ResourceType>> GetResourceType(string name)
        {
            ResourceType resourceType;
            if (!ResourceTypes.TryGetValue(name, out resourceType))
                return Task.FromResult<IScimResponse<ResourceType>>(
                    new ScimErrorResponse<ResourceType>(
                        new ScimError(
                            HttpStatusCode.NotFound)));

            return Task.FromResult<IScimResponse<ResourceType>>(
                new ScimDataResponse<ResourceType>(resourceType));
        }

        private IReadOnlyDictionary<string, ResourceType> CreateResourceTypes()
        {
            return ServerConfiguration.ResourceTypeDefinitions
                .ToDictionary(
                    rtd => rtd.Name,
                    rtd => SetResourceVersion(
                        new ResourceType
                        {
                            Description = rtd.Description,
                            Schema = rtd.Schema,
                            Name = rtd.Name,
                            Endpoint = rtd.Endpoint,
                            SchemaExtensions = rtd.SchemaExtensions
                                .Select(ext => new SchemaExtension(ext.Schema, ext.Required))
                                .ToList()
                        }),
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}