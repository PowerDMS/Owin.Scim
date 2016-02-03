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
            Name = name;
            Description = description;
            SpecUri = specUri;
            DocumentationUri = documentationUri;
            Type = type;
            Primary = isPrimary;
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public Uri SpecUri { get; set; }

        public Uri DocumentationUri { get; set; }
    }
}