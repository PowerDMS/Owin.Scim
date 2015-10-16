namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public abstract class MultiValuedAttribute
    {
        public string Type { get; set; }

        public bool Primary { get; set; }

        public string Display { get; set; }

        public string Value { get; set; }

        [JsonProperty("$ref")]
        public string Ref { get; set; }
    }
}