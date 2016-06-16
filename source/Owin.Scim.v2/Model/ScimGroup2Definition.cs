namespace Owin.Scim.v2.Model
{
    using Canonicalization;

    using Configuration;

    using Extensions;

    using Validation.Groups;

    public class ScimGroup2Definition : ScimResourceTypeDefinitionBuilder<ScimGroup2>
    {
        public ScimGroup2Definition(ScimServerConfiguration serverConfiguration)
            : base(
                serverConfiguration,
                ScimConstants.ResourceTypes.Group,
                ScimConstantsV2.Schemas.Group,
                ScimConstantsV2.Endpoints.Groups,
                typeof (ScimGroup2Validator),
                schemaIdentifiers => schemaIdentifiers.Contains(ScimConstantsV2.Schemas.Group))
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