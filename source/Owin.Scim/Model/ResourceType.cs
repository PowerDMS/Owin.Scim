namespace Owin.Scim.Model
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public sealed class ResourceType : Resource
    {
        public ResourceType()
        {
            AddSchema(ScimConstants.Schemas.ResourceType);
            Meta.ResourceType = ScimConstants.ResourceTypes.ResourceType;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("schema")]
        public string Schema { get; set; }

        [JsonProperty("schemaExtensions")]
        public IEnumerable<SchemaExtension> SchemaExtensions { get; set; }

        public override string GenerateETagHash()
        {
            return null;
        }

        public override bool ShouldSerializeId()
        {
            return false;
        }
    }
}