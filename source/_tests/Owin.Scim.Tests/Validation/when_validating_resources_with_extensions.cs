namespace Owin.Scim.Tests.Validation
{
    using System.Threading;

    using Configuration;

    using FakeItEasy;

    using FluentValidation;

    using Machine.Specifications;
    
    using Model.Users;

    using Repository;

    using Scim.Extensions;
    using Scim.Security;
    using Scim.Validation;

    using v2.Model;
    using v2.Validation;

    public class when_validating_resources_with_extensions
    {
        Establish context = async () =>
        {
            var serverConfiguration = A.Fake<ScimServerConfiguration>();
            var userRepository = A.Fake<IUserRepository>();
            var passwordManager = A.Fake<IManagePasswords>();
            var validatorFactory = A.Fake<IResourceValidatorFactory>();

            _ExtensionValidator = A.Fake<IResourceExtensionValidator>(b => b.Wrapping(new EnterpriseUser2ExtensionValidator()));

            A.CallTo(() => validatorFactory.CreateValidator(A<ScimUser>._))
                .Returns(
                    new ScimUser2Validator(
                        serverConfiguration,
                        new ResourceExtensionValidators(
                            new []
                            {
                                _ExtensionValidator
                            }),
                        userRepository,
                        passwordManager));


            _UserValidator = await validatorFactory.CreateValidator(User).AwaitResponse().AsTask;

            User = new ScimUser2();
            User.Extension<EnterpriseUser2Extension>().Department = "sales";
        };

        Because of = async () => (await _UserValidator.ValidateCreateAsync(User).AwaitResponse().AsTask)
            .ToScimValidationResult();

        It should_have_invoked_the_extension_validator = 
            () => 
            A.CallTo(() => _ExtensionValidator.ValidateAsync(A<ValidationContext>._, A<CancellationToken>._))
             .MustHaveHappened();
        
        protected static ScimUser User;

        private static IValidator _UserValidator;

        private static IResourceExtensionValidator _ExtensionValidator;
    }
}