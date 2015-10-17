namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    using Repository;

    using Scim.Validation;
    using Scim.Validation.Users;

    using Security;

    public abstract class when_validating_a_new_user
    {
        Establish context = () =>
        {
            UserRepository = A.Fake<IUserRepository>();
            PasswordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();

            _Validator = new UserValidator(UserRepository, PasswordComplexityVerifier);
        };

        Because of = async () => Result = await _Validator.ValidateAsync(User, RuleSetConstants.Create).AwaitResponse().AsTask;

        protected static IUserRepository UserRepository;

        protected static IVerifyPasswordComplexity PasswordComplexityVerifier;

        protected static User User;

        protected static ValidationResult Result;

        private static UserValidator _Validator;
    }
}