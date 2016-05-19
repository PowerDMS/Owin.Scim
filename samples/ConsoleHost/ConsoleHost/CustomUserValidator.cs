namespace ConsoleHost
{
    using System.Net;

    using FluentValidation;

    using Owin.Scim.Configuration;
    using Owin.Scim.ErrorHandling;
    using Owin.Scim.Model;
    using Owin.Scim.Repository;
    using Owin.Scim.Security;
    using Owin.Scim.Validation;
    using Owin.Scim.Validation.Users;

    public class CustomUserValidator : UserValidator
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