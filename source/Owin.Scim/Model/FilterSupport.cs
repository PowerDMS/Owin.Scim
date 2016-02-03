namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public class FilterSupport
    {
        public bool Supported
        {
            get { return false; }
        }

        [JsonProperty("maxResults")]
        public int MaxResults
        {
            get { return 1; } }
    }
}