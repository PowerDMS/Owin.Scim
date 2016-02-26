namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using FluentValidation;

    using Machine.Specifications;

    using Model.Users;

    using Repository;

    using Scim.Extensions;
    using Scim.Validation;
    using Scim.Validation.Users;

    using Security;

    public class when_validating_a_user
    {
        Establish context = () =>
        {
            UserRepository = A.Fake<IUserRepository>();
            PasswordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();
            PasswordManager = A.Fake<IManagePasswords>();

            _ValidatorFactory = new ResourceValidatorFactory(UserRepository, PasswordComplexityVerifier, PasswordManager);

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(true);
        };

        Because of = async () =>
        {
            _Validator = await _ValidatorFactory.CreateValidator(User);
            Result = (await _Validator.ValidateAsync(User, ruleSet: RuleSetConstants.Default).AwaitResponse().AsTask).ToScimValidationResult();
        };

        protected static IUserRepository UserRepository;

        protected static IVerifyPasswordComplexity PasswordComplexityVerifier;

        protected static IManagePasswords PasswordManager;

        protected static User User;

        protected static ValidationResult Result;

        private static IValidator _Validator;

        private static ResourceValidatorFactory _ValidatorFactory;
    }
}
