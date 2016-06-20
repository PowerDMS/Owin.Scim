namespace Owin.Scim.v1.Model
{
    using Canonicalization;

    using Configuration;

    using Extensions;

    using Validation;

    public class ScimGroup1Definition : ScimResourceTypeDefinitionBuilder<ScimGroup1>
    {
        public ScimGroup1Definition(ScimServerConfiguration serverConfiguration)
            : base(
                serverConfiguration,
                ScimConstants.ResourceTypes.Group,
                ScimConstantsV1.Schemas.Group,
                ScimConstantsV1.Endpoints.Groups,
                typeof (ScimGroup1Validator),
                (schemaIdentifiers, parameterType) => parameterType == typeof(ScimGroup1))
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