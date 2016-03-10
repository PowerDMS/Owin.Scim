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
        private const string PathSeparator = @"/";
        private readonly ScimServerConfiguration _scimServerConfiguration;
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;

        public GroupValidator(
            ResourceExtensionValidators extensionValidators,
            ScimServerConfiguration scimServerConfiguration,
            IUserRepository userRepository, 
            IGroupRepository groupRepository)
            : base(extensionValidators)
        {
            _scimServerConfiguration = scimServerConfiguration;
            _userRepository = userRepository;
            _groupRepository = groupRepository;
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

            When(@group => @group.Members != null && @group.Members.Any(), () =>
                {
                    RuleFor(@group => @group.Members)
                        .SetCollectionValidator(
                            new GenericExpressionValidator<Member>
                            {
                                {
                                    g => g.Value,
                                    config => config
                                        .NotEmpty()
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                ErrorDetail.AttributeRequired("member.value")))
                                },
                                {
                                    g => g.Type,
                                    config => config
                                        .NotEmpty()
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidValue,
                                                ErrorDetail.AttributeRequired("member.type")))
                                        .Must(IsValidMemberType)
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.type' must have a valid value."))
                                },
                                {
                                    g => g,
                                    config => config
                                        .MustAsync(async (member, token) => await IsValidResourceValue(member))
                                        .WithState(u =>
                                            new ScimError(
                                                HttpStatusCode.BadRequest,
                                                ScimErrorType.InvalidSyntax,
                                                "The attribute 'member.$ref' (or 'member.value' and 'member.type') must be a valid resource."))
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
                var user = await _userRepository.GetUser(member.Value);
                return user != null;
            }

            if (member.Type == ScimConstants.ResourceTypes.Group)
            {
                var group = await _groupRepository.GetGroup(member.Value);
                return group != null;
            }

            return false;
        }
    }
}