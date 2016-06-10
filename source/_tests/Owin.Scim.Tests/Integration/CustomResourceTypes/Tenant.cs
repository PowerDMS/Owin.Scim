namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using Model;

    using Newtonsoft.Json;

    public class Tenant : Resource
    {
        public override string SchemaIdentifier
        {
            get { return CustomSchemas.Tenant; }
        }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}