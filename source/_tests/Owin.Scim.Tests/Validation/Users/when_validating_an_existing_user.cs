namespace Owin.Scim.Tests.Validation.Users
{
    using Configuration;

    using FakeItEasy;

    using FluentValidation;

    using Machine.Specifications;

    using Model.Users;

    using Repository;

    using Scim.Extensions;
    using Scim.Security;
    using Scim.Validation;

    public class when_validating_an_existing_user
    {
        Establish context = () =>
        {
            ServerConfiguration = A.Fake<ScimServerConfiguration>();
            UserRepository = A.Fake<IUserRepository>();
            PasswordManager = A.Fake<IManagePasswords>();

            _ValidatorFactory = new UserValidatorFactory(ServerConfiguration, UserRepository, PasswordManager);

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(true);
        };

        Because of = async () =>
        {
            _Validator = await _ValidatorFactory.CreateValidator(User);
            Result = (await _Validator.ValidateUpdateAsync(User, ExistingUserRecord).AwaitResponse().AsTask)
                .ToScimValidationResult();
        };

        protected static ScimUser ExistingUserRecord;

        protected static ScimServerConfiguration ServerConfiguration;

        protected static IUserRepository UserRepository;
        
        protected static IManagePasswords PasswordManager;

        protected static ScimUser User;

        protected static ValidationResult Result;

        private static IValidator _Validator;

        private static UserValidatorFactory _ValidatorFactory;
    }
}