namespace ConsoleHost.Scim
{
    using System.Net;

    using FluentValidation;

    using Owin.Scim.Configuration;
    using Owin.Scim.ErrorHandling;
    using Owin.Scim.Model;
    using Owin.Scim.Repository;
    using Owin.Scim.Security;
    using Owin.Scim.v2.Validation.Users;
    using Owin.Scim.Validation;

    public class CustomUserValidator : ScimUser2Validator
    {
        public CustomUserValidator(
            ScimServerConfiguration serverConfiguration, 
            ResourceExtensionValidators extensionValidators, 
            IUserRepository userRepository, 
            IManagePasswords passwordManager) 
            : base(serverConfiguration, extensionValidators, userRepository, passwordManager)
        {
        }

        protected override void ConfigureDefaultRuleSet()
        {
            base.ConfigureDefaultRuleSet();

            RuleFor(u => u.NickName)
                .NotNull()
                .WithState(u =>
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidValue,
                        ScimErrorDetail.AttributeRequired("nickName")));
        }
    }
}