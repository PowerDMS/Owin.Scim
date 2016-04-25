namespace Owin.Scim.Model
{
    using Configuration;

    public abstract class ResourceExtension
    {
        [ScimInternal]
        protected internal abstract string SchemaIdentifier { get; }

        public abstract int CalculateVersion();
    }
}