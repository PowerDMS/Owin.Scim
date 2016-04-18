namespace Owin.Scim.Model
{
    [ScimTypeDefinition(typeof(ScimFeatureDefinition))]
    public class ScimFeature
    {
        public ScimFeature(bool supported)
        {
            Supported = supported;
        }

        public bool Supported { get; private set; }

        protected internal virtual int GetETagHashCode()
        {
            return Supported.GetHashCode();
        }
    }
}