namespace Owin.Scim.Model.Groups
{
    using System;

    using Configuration;

    public class MemberDefinition : ScimTypeDefinitionBuilder<Member>
    {
        public MemberDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(p => p.Value)
                .SetDescription("Identifier of the member of this group.")
                .SetMutability(Mutability.Immutable);

            For(p => p.Ref)
                .SetDescription("The URI corresponding to a SCIM resource that is a member of this group.")
                .SetMutability(Mutability.Immutable)
                .SetReferenceTypes(ScimConstants.ResourceTypes.User, ScimConstants.ResourceTypes.Group);

            For(p => p.Type)
                .SetCanonicalValues(new [] { ScimConstants.ResourceTypes.User, ScimConstants.ResourceTypes.Group }, StringComparer.OrdinalIgnoreCase)
                .SetDescription("A label indicating the type of resource, e.g., 'User' or 'Group'.")
                .SetMutability(Mutability.Immutable);
        }
    }
}