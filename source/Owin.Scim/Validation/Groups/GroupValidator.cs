namespace Owin.Scim.Validation.Groups
{
    using System;
    using System.Collections.Generic;
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
                                        g => g.Type,
                                        config => config
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
                                            .Must(IsResourceProvided)
                                            .WithState(u =>
                                                new ScimError(
                                                    HttpStatusCode.BadRequest,
                                                    ScimErrorType.InvalidValue,
                                                    "The attribute 'member.$ref' (or 'member.value' and 'member.type') must be provided."))
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

        /// <summary>
        /// for members, I want to let clients pass me $ref or value/type, if both are passed
        /// $ref takes precedence
        /// </summary>
        private bool IsResourceProvided(Member member)
        {
            return member.Ref != null || (!string.IsNullOrWhiteSpace(member.Value) && !string.IsNullOrWhiteSpace(member.Type));
        }

        protected virtual bool IsValidMemberType(string type)
        {
            return type == null || 
                type.Equals(ScimConstants.ResourceTypes.User) || 
                type.Equals(ScimConstants.ResourceTypes.Group);
        }

        private async Task<bool> IsValidResourceValue(Member member)
        {
            if (member.Ref != null)
            {
                if (member.Ref.IsAbsoluteUri)
                {
                    string type, value;
                    var publicOrigin = new Uri(_scimServerConfiguration.PublicOrigin);

                    if (TryReadTypeAndValue(publicOrigin, member.Ref, out type, out value))
                    {
                        if (string.Compare(type, ScimConstants.ResourceTypes.User,
                            StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            var user = await _userRepository.GetUser(value);
                            return user != null;
                        }
                        if (string.Compare(type, ScimConstants.ResourceTypes.Group,
                            StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            var group = await _groupRepository.GetGroup(value);
                            return group != null;
                        }
                    }
                }
                else
                {
                    // TODO: (CY) in order to support relative, I need the request query string
                    return false;
                }
            }
            else
            {
                if (string.Compare(member.Type, ScimConstants.ResourceTypes.User,
                        StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var user = await _userRepository.GetUser(member.Value);
                    return user != null;
                }
                if (string.Compare(member.Type, ScimConstants.ResourceTypes.Group,
                        StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    var group = await _groupRepository.GetGroup(member.Value);
                    return group != null;
                }
            }

            return false;
        }

        private bool TryReadTypeAndValue(Uri rootUri, Uri uri, out string type, out string value)
        {
            type = value = null;

            if (rootUri.Host != uri.Host) return false;

            if (rootUri.Segments.Length != uri.Segments.Length - 2) return false;

            var prefixPath = uri.Segments.ToList().GetRange(0, rootUri.Segments.Length);

            if (!rootUri.Segments.SequenceEqual(prefixPath, StringComparer.InvariantCultureIgnoreCase)) return false;

            var endpoint = uri.Segments[uri.Segments.Length - 2].Replace(PathSeparator, string.Empty).ToLower();
            value = uri.Segments[uri.Segments.Length - 1].Replace(PathSeparator, string.Empty);

            return ScimConstants.Maps.EndpointToTypeDictionary.TryGetValue(endpoint, out type);
        }
    }
}