namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public class ResourceType : Resource
    {
        public ResourceType()
        {
            AddSchema(ScimConstants.Schemas.ResourceType);
            Meta.ResourceType = ScimConstants.ResourceTypes.ResourceType;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

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