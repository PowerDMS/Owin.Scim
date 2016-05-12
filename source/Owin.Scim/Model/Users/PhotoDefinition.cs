namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;
    using Canonicalization;

    public class PhotoDefinition : ScimTypeDefinitionBuilder<Photo>
    {
        public PhotoDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(photo => photo.Value)
                .SetDescription(@"URL of a photo of the user.")
                .SetReferenceTypes(ScimConstants.ReferenceTypes.External)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
            
            For(photo => photo.Type)
                .SetDescription(@"A label indicating the attribute's function, i.e., 'photo' or 'thumbnail'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.PhotoTypes, StringComparer.OrdinalIgnoreCase)
                .AddCanonicalizationRule(type => type.ToLower());

            For(photo => photo.Primary)
                .SetDescription(
                    @"A Boolean value indicating the 'primary' or preferred 
                      attribute value for this attribute, e.g., the preferred photo or thumbnail.");

            For(photo => photo.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}