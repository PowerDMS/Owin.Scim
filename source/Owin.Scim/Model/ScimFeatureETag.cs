namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    public class ScimFeatureETag : ScimFeature
    {
        public ScimFeatureETag(bool supported, bool isWeak)
            : base(supported)
        {
            IsWeak = isWeak;
        }

        [JsonIgnore]
        public bool IsWeak { get; private set; }
    }
}