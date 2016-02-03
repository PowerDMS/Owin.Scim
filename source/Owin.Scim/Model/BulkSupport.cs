namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public class BulkSupport
    {
        public bool Supported
        {
            get { return false; }
        }

        [JsonProperty("maxOperations")]
        public int MaxOperations
        {
            get { return 1; }
        }

        [JsonProperty("maxPayloadSize")]
        public int MaxPayloadSize
        {
            get { return 1; }
        }
    }
}