namespace Owin.Scim.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    [ScimTypeDefinition(typeof(ResourceTypeDefinition))]
    public sealed class ResourceType : Resource
    {
        public ResourceType()
        {
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.ResourceType);
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

        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.ResourceType; }
        }

        public override int CalculateVersion()
        {
            return new
            {
                Name,
                Description,
                Endpoint,
                Schema,
                SchemaExtensions =
                    SchemaExtensions?.Aggregate(
                        0,
                        (hashSeed, se) =>
                        {
                            if (se == null) return 0;

                            return hashSeed * 31 + new { se.Schema, se.Required }.GetHashCode();
                        })
            }.GetHashCode();
        }
    }
}