namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public class ScimFeatureBulk : ScimFeature
    {
        private ScimFeatureBulk(int maxOperations, int maxPayloadSize)
            : base(true)
        {
            MaxOperations = maxOperations;
            MaxPayloadSize = maxPayloadSize;
        }

        private ScimFeatureBulk()
            : base(false)
        {
        }

        [JsonProperty("maxOperations")]
        public int MaxOperations { get; private set; }

        [JsonProperty("maxPayloadSize")]
        public int MaxPayloadSize { get; private set; }

        public static ScimFeature Create(int maxOperations, int maxPayloadSizeInBytes)
        {
            return new ScimFeatureBulk(maxOperations, maxPayloadSizeInBytes);
        }

        public static ScimFeature CreateUnsupported()
        {
            return new ScimFeatureBulk();
        }

        protected internal override int GetETagHashCode()
        {
            unchecked
            {
                var hash = 19;
                hash = hash * 31 + base.GetETagHashCode();
                hash = hash * 31 + new
                {
                    MaxOperations,
                    MaxPayloadSize
                }.GetHashCode();

                return hash;
            }
        }
    }
}