namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    [ScimTypeDefinition(typeof(ScimFeatureDefinition))]
    public class ScimFeature
    {
        public ScimFeature(bool supported)
        {
            Supported = supported;
        }

        [JsonProperty("supported")]
        public bool Supported { get; private set; }

        protected internal virtual int GetETagHashCode()
        {
            return Supported.GetHashCode();
        }
    }
}