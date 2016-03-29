namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using FluentValidation;

    using Machine.Specifications;

    using Model.Users;

    using Repository;

    using Scim.Extensions;
    using Scim.Security;
    using Scim.Validation;
    
    public class when_validating_a_new_user
    {
        Establish context = () =>
        {
            UserRepository = A.Fake<IUserRepository>();
            PasswordManager = A.Fake<IManagePasswords>();

            _ValidatorFactory = new UserValidatorFactory(UserRepository, PasswordManager);

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(true);
        };

        Because of = async () =>
        {
            _Validator = await _ValidatorFactory.CreateValidator(User);
            Result = (await _Validator.ValidateCreateAsync(User).AwaitResponse().AsTask).ToScimValidationResult();
        };

        protected static IUserRepository UserRepository;

        protected static IManagePasswords PasswordManager;

        protected static User User;

        protected static ValidationResult Result;

        private static IValidator _Validator;

        private static UserValidatorFactory _ValidatorFactory;
    }
}