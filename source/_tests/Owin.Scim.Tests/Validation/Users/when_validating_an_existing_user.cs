namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    using Repository;

    using Scim.Validation;
    using Scim.Validation.Users;

    using Security;

    public abstract class when_validating_an_existing_user
    {
        Establish context = () =>
        {
            UserRepository = A.Fake<IUserRepository>();
            PasswordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();
            PasswordManager = A.Fake<IManagePasswords>();

            _Validator = new UserValidator(UserRepository, PasswordComplexityVerifier, PasswordManager);

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(true);
        };

        Because of = async () => Result = await _Validator.ValidateAsync(User, RuleSetConstants.Update).AwaitResponse().AsTask;

        protected static IUserRepository UserRepository;

        protected static IVerifyPasswordComplexity PasswordComplexityVerifier;

        protected static IManagePasswords PasswordManager;

        protected static User User;

        protected static ValidationResult Result;

        private static UserValidator _Validator;
    }
}