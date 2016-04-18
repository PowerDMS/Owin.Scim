namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class PhotoDefinition : ScimTypeDefinitionBuilder<Photo>
    {
        public PhotoDefinition()
        {
            For(p => p.Value)
                .SetDescription(@"URL of a photo of the user.")
                .SetReferenceTypes(ScimConstants.ReferenceTypes.External);

            For(p => p.Type)
                .SetDescription(@"A label indicating the attribute's function, i.e., 'photo' or 'thumbnail'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.PhotoTypes, StringComparer.OrdinalIgnoreCase)
                .AddCanonicalizationRule(type => type.ToLower());

            For(p => p.Primary)
                .SetDescription(
                    @"A Boolean value indicating the 'primary' or preferred 
                      attribute value for this attribute, e.g., the preferred photo or thumbnail.");
        }
    }
}