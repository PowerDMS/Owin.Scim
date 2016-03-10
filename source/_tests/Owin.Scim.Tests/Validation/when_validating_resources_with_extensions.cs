namespace Owin.Scim.Tests.Validation
{
    using System.Threading;

    using FakeItEasy;

    using FluentValidation;

    using Machine.Specifications;
    
    using Model.Users;

    using Repository;

    using Scim.Extensions;
    using Scim.Validation;
    using Scim.Validation.Users;

    using Security;

    public class when_validating_resources_with_extensions
    {
        Establish context = async () =>
        {
            var userRepository = A.Fake<IUserRepository>();
            var passwordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();
            var passwordManager = A.Fake<IManagePasswords>();
            var validatorFactory = A.Fake<IResourceValidatorFactory>();

            _ExtensionValidator = A.Fake<IResourceExtensionValidator>(b => b.Wrapping(new EnterpriseUserExtensionValidator()));

            A.CallTo(() => validatorFactory.CreateValidator(A<User>._))
                .Returns(
                    new UserValidator(
                        new ResourceExtensionValidators(
                            new []
                            {
                                _ExtensionValidator
                            }),
                        userRepository,
                        passwordComplexityVerifier,
                        passwordManager));


            _UserValidator = await validatorFactory.CreateValidator(User).AwaitResponse().AsTask;

            User = new User();
            User.Extension<EnterpriseUserExtension>().Department = "sales";
        };

        Because of = async () => (await _UserValidator.ValidateCreateAsync(User).AwaitResponse().AsTask)
            .ToScimValidationResult();

        It should_have_invoked_the_extension_validator = 
            () => 
            A.CallTo(() => _ExtensionValidator.ValidateAsync(A<ValidationContext>._, A<CancellationToken>._))
             .MustHaveHappened();
        
        protected static User User;

        private static IValidator _UserValidator;

        private static IResourceExtensionValidator _ExtensionValidator;
    }
}