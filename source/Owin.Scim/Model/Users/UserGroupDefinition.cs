namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class UserGroupDefinition : MultiValuedAttributeDefinition
    {
        public UserGroupDefinition()
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
                .SetMutability(Mutability.ReadOnly);

            For(g => g.Primary)
                .SetMutability(Mutability.ReadOnly);
        }

        public override Type DefinitionType
        {
            get { return typeof(UserGroup); }
        }
    }
}