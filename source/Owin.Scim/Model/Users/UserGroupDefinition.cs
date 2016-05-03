namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class UserGroupDefinition : ScimTypeDefinitionBuilder<UserGroup>
    {
        public UserGroupDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(g => g.Value)
                .SetDescription(@"The identifier of the user's group.")
                .SetMutability(Mutability.ReadOnly);

            For(g => g.Ref)
                .SetDescription(@"The URI of the corresponding 'Group' resource to which the user belongs.")
                .SetReferenceTypes(ScimConstants.ResourceTypes.Group)
                .SetMutability(Mutability.ReadOnly);

            For(g => g.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'direct' or 'indirect'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.UserGroupTypes)
                .SetMutability(Mutability.ReadOnly);

            For(g => g.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);

            For(g => g.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.")
                .SetMutability(Mutability.ReadOnly);
        }
    }
}