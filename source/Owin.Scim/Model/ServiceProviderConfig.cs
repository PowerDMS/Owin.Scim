namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    public class ServiceProviderConfig : Resource
    {
        private readonly PatchSupprt _Patch = new PatchSupprt();

        private readonly BulkSupport _Bulk = new BulkSupport();

        private readonly FilterSupport _Filter = new FilterSupport();

        private readonly ChangePasswordSupport _ChangePassword = new ChangePasswordSupport();

        private readonly SortSupport _Sort = new SortSupport();

        private readonly ETagSupport _ETag = new ETagSupport();

        private readonly IEnumerable<AuthenticationScheme> _AuthenticationSchemes; 

        public ServiceProviderConfig(IEnumerable<AuthenticationScheme> authenticationSchemes = null)
        {
            _AuthenticationSchemes = authenticationSchemes?.ToList() ?? new List<AuthenticationScheme>();

            AddSchema(ScimConstants.Schemas.ServiceProviderConfig);
            Meta.ResourceType = ScimConstants.ResourceTypes.ServiceProviderConfig;
        }

        [JsonProperty("name")]
        public string Name
        {
            get
            {
                return @"Service Provider Configuration";
            }
        }

        [JsonProperty("description")]
        public string Description
        {
            get
            {
                return @"Schema for representing the service provider's configuration";
            }
        }

        [JsonProperty("documentationUri")]
        public string DocumentationUri
        {
            get { return null; }
        }

        [JsonProperty("patch")]
        public PatchSupprt Patch { get { return _Patch; } }

        [JsonProperty("bulk")]
        public BulkSupport Bulk { get { return _Bulk; } }

        [JsonProperty("filter")]
        public FilterSupport Filter { get { return _Filter; } }

        [JsonProperty("changePassword")]
        public ChangePasswordSupport ChangePassword { get { return _ChangePassword; } }

        [JsonProperty("sort")]
        public SortSupport Sort { get { return _Sort; } }

        [JsonProperty("etag")]
        public ETagSupport ETag { get { return _ETag; } }

        [JsonProperty("authenticationSchemes")]
        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }
    }
}