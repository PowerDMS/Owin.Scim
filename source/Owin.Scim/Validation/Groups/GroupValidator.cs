namespace Owin.Scim.Validation.Groups
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using FluentValidation;

    using Configuration;
    using ErrorHandling;
    using Model;
    using Model.Groups;
    using Repository;

    public class GroupValidator : ResourceValidatorBase<Group>
    {
        private readonly IUserRepository _UserRepository;

        private readonly IGroupRepository _GroupRepository;

        public GroupValidator(
            ResourceExtensionValidators extensionValidators,
            IUserRepository userRepository,
            IGroupRepository groupRepository)
            : base(extensionValidators)
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
                        ErrorDetail.AttributeRequired("displayName")));

            // SCIM specification does not specify whether to use value/type or $ref
            When(group => group.Members != null && group.Members.Any(),
                () =>
                {
                    RuleFor(group => group.Members)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<Member>
                            {
                                {
                                    member => member.Value,
                                    config => config
                                        .NotEmpty()
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                ErrorDetail.AttributeRequired("member.value")))
                                },
                                {
                                    member => member.Type,
                                    config => config
                                        .NotEmpty()
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                ErrorDetail.AttributeRequired("member.type")))
                                        .Must(IsValidMemberType)
                                        .WithState(m =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.type' must have a valid value."))
                                },
                                {
                                    member => member.Ref,
                                    config => config
                                        .Must(uri => uri == null || uri.IsWellFormedOriginalString()) // TODO: (DG) not valid uri validation
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.$ref' must have a valid url."))
                                },
                                {
                                    member => member,
                                    config => config
                                        .MustAsync(async (member, token) => await IsValidResourceValue(member))
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.value' and 'member.type' must be a valid resource."))
                                }
                            });
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
                var user = await _UserRepository.GetUser(member.Value);
                return user != null;
            }

            if (member.Type == ScimConstants.ResourceTypes.Group)
            {
                var group = await _GroupRepository.GetGroup(member.Value);
                return group != null;
            }

            return false;
        }
    }
}