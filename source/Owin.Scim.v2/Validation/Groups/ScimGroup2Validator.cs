namespace Owin.Scim.v2.Validation.Groups
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Configuration;

    using ErrorHandling;

    using Extensions;

    using FluentValidation;

    using Model;

    using Repository;

    using Scim.Model;
    using Scim.Model.Groups;
    using Scim.Validation;

    public class ScimGroup2Validator : ResourceValidatorBase<ScimGroup2>
    {
        private readonly IUserRepository _UserRepository;

        private readonly IGroupRepository _GroupRepository;

        public ScimGroup2Validator(
            ScimServerConfiguration serverConfiguration,
            ResourceExtensionValidators extensionValidators,
            IUserRepository userRepository,
            IGroupRepository groupRepository)
            : base(serverConfiguration, extensionValidators)
        {
            _UserRepository = userRepository;
            _GroupRepository = groupRepository;
        }

        protected override void ConfigureDefaultRuleSet()
        {
            RuleFor(g => g.DisplayName)
                .NotEmpty()
                .WithState(u =>
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidValue,
                        ScimErrorDetail.AttributeRequired("displayName")));
            
            // Owin.Scim group membership requires clients to either specify a member.Value & member.Type
            // or a member.Ref URI.
            When(group => group.Members != null && group.Members.Any(),
                () =>
                {
                    RuleFor(group => group.Members)
                        .NestedRules(v =>
                        {
                            v.When(
                                m =>
                                    string.IsNullOrWhiteSpace(m.Value) &&
                                    m.Ref == null,
                                () =>
                                    v.RuleFor(m => m.Value)
                                        .NotEmpty()
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                "To modify group members you must specify either a ('member.value' and 'member.type') combination or a ('member.ref') to a valid resource.")));

                            v.When(
                                m => !string.IsNullOrWhiteSpace(m.Value),
                                () =>
                                {
                                    v.RuleFor(m => m.Value)
                                        .NotEmpty()
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                ScimErrorDetail.AttributeRequired("member.value")));
                                    v.RuleFor(m => m.Type)
                                        .NotEmpty()
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                ScimErrorDetail.AttributeRequired("member.type")))
                                        .Must(IsValidMemberType)
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.type' must have a valid value."));
                                    v.RuleFor(m => m)
                                        .MustAsync(async (member, token) => await IsValidResourceValue(member))
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.value' and 'member.type' must be a valid resource."));
                                });

                            v.When(
                                m => string.IsNullOrWhiteSpace(m.Value),
                                () =>
                                {
                                    v.RuleFor(m => m.Ref)
                                        .NotNull()
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.$ref' must have a valid a url."))
                                        .Must(uri => uri.IsAbsoluteUri)
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.$ref' must have a valid url."));

                                    v.RuleFor(m => m)
                                        .MustAsync(EnsureMemberReferenceIsValid)
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.$ref' must have a valid url."));
                                });
                        });
                });
        }

        private Task<bool> EnsureMemberReferenceIsValid(Member member, CancellationToken token)
        {
            var typeDefinition = ServerConfiguration.GetScimTypeDefinition(typeof(Member));
            var attributeDefinition = typeDefinition.GetAttributeDefinition(() => member.Ref);

            if (!member.Ref.IsScimServerUri())
                return Task.FromResult(false);

            var scimReference = member.Ref.ToScimResourceReference(ServerConfiguration);
            if (scimReference == null)
                return Task.FromResult(false); // invalid SCIM resource reference URI

            // if the attribute definition has reference types, validate that the resource reference is allowed
            // e.g. "User", "Group"
            // this is obtained from the ResourceDefinition.Name
            if (attributeDefinition.ReferenceTypes != null &&
                attributeDefinition.ReferenceTypes.Any() &&
                !attributeDefinition.ReferenceTypes.Contains(
                    scimReference.ResourceDefinition.Name,
                    StringComparer.OrdinalIgnoreCase))
            {
                return Task.FromResult(false);
            }

            // ensure the resource exists
            return IsValidResourceValue(
                new Member
                {
                    Value = scimReference.ResourceId,
                    Type = scimReference.ResourceDefinition.Name
                });
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }

        protected virtual bool IsValidMemberType(string type)
        {
            return type == null ||
                type.Equals(ScimConstants.ResourceTypes.User) ||
                type.Equals(ScimConstants.ResourceTypes.Group);
        }

        protected virtual async Task<bool> IsValidResourceValue(Member member)
        {
            if (member.Type == ScimConstants.ResourceTypes.User)
            {
                var userExists = await _UserRepository.UserExists(member.Value);
                return userExists;
            }

            if (member.Type == ScimConstants.ResourceTypes.Group)
            {
                var groupExists = await _GroupRepository.GroupExists(member.Value);
                return groupExists;
            }

            return false;
        }
    }
}