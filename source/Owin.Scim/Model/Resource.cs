namespace Owin.Scim.Model
{
    using System.ComponentModel;

    using Newtonsoft.Json;

    public abstract class Resource : SchemaBase
    {
        [Description(@"A unique identifier for a SCIM resource as defined by the service provider.")]
        [JsonProperty(Order = -5, PropertyName = "id")]
        public string Id { get; set; }
        
        [Description(@"An identifier for the resource as defined by the provisioning client.")]
        [JsonProperty(PropertyName = "externalId")]
        public string ExternalId { get; set; }

        public ResourceMetadata Meta { get; set; }

        public abstract string CalculateVersion();

        public virtual bool ShouldSerializeId()
        {
            return true;
        }

        public virtual bool ShouldSerializeExternalId()
        {
            return true;
        }
    }
}