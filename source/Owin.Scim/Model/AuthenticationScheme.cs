namespace Owin.Scim.Model
{
    using System;

    public class AuthenticationScheme : MultiValuedAttribute
    {
        public AuthenticationScheme(
            string type, 
            string name, 
            string description,
            Uri specUri = null,
            Uri documentationUri = null,
            bool isPrimary = false)
        {
            Type = type;
            Primary = isPrimary;
            Name = name;
            Description = description;
            SpecUri = specUri;
            DocumentationUri = documentationUri;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public Uri SpecUri { get; private set; }

        public Uri DocumentationUri { get; private set; }

        protected internal override int CalculateVersion()
        {
            return new
            {
                Base = base.CalculateVersion(),
                Name,
                Description,
                SpecUri,
                DocumentationUri
            }.GetHashCode();
        }
    }
}