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
            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(u => u.Id)
                .SetMutability(Mutability.ReadOnly)
                .SetReturned(Returned.Always)
                .SetUniqueness(Uniqueness.Server)
                .SetCaseExact(true);

            For(u => u.DisplayName)
                .SetRequired(true);

            For(u => u.Members)
                .AddCanonicalizationRule(member => member.Canonicalize(e => e.Type, StringExtensions.UppercaseFirstCharacter));

            For(u => u.Meta)
                .SetMutability(Mutability.ReadOnly)
                .SetReturned(Returned.Always);
        }
    }
}