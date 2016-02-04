namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public abstract class Resource : SchemaBase
    {
        protected Resource()
        {
            Meta = new ResourceMetadata();
        }

        [JsonProperty(Order = -5, PropertyName = "id")]
        public string Id { get; set; }

        public ResourceMetadata Meta { get; set; }
    }
}