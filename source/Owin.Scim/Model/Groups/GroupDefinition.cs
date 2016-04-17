namespace Owin.Scim.Model.Groups
{
    using Canonicalization;

    using Configuration;

    using Extensions;

    using Validation.Groups;

    public class GroupDefinition : ScimResourceTypeDefinitionBuilder<Group>
    {
        public GroupDefinition()
            : base(
                ScimConstants.ResourceTypes.Group,
                ScimConstants.Schemas.Group,
                ScimConstants.Endpoints.Groups,
                typeof(GroupValidator),
                schemaIdentifiers => schemaIdentifiers.Contains(ScimConstants.Schemas.Group))
        {
            SetName("Group");
            SetDescription("Group resource.");

            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(u => u.Id)
                .SetMutability(Mutability.ReadOnly)
                .SetReturned(Returned.Always)
                .SetUniqueness(Uniqueness.Server)
                .SetCaseExact(true);

            For(u => u.DisplayName)
                .SetDescription("A human-readable name for the group.")
                .SetRequired(true);

            For(u => u.Members)
                .SetDescription("A list of members of the group.")
                .AddCanonicalizationRule(member => member.Canonicalize(e => e.Type, StringExtensions.UppercaseFirstCharacter));

            For(u => u.Meta)
                .SetMutability(Mutability.ReadOnly)
                .SetReturned(Returned.Always);
        }
    }
}