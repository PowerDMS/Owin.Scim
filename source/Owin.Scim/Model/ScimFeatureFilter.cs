namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public class ScimFeatureFilter : ScimFeature
    {
        private ScimFeatureFilter(int maxResults)
            : base(true)
        {
            MaxResults = maxResults;
        }

        private ScimFeatureFilter()
            : base(false)
        {
        }

        [JsonProperty("maxResults")]
        public int MaxResults { get; private set; }

        public static ScimFeature Create(int maxResults)
        {
            return new ScimFeatureFilter(maxResults);
        }

        public static ScimFeature CreateUnsupported()
        {
            return new ScimFeatureFilter();
        }

        protected internal override int GetETagHashCode()
        {
            unchecked
            {
                var hash = 19;
                hash = hash * 31 + base.GetETagHashCode();
                hash = hash * 31 + new
                {
                    MaxResults
                }.GetHashCode();

                return hash;
            }
        }
    }
}