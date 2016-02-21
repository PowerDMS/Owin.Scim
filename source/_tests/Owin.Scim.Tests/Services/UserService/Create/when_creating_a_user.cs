namespace Owin.Scim.Tests.Services.UserService.Create
{
    using System;
    using System.Threading.Tasks;

    using Canonicalization;

    using Configuration;

    using FakeItEasy;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    using Repository;

    using Scim.Services;
    using Scim.Validation.Users;

    using Security;

    public class when_creating_a_user
    {
        Establish context = () =>
        {
            ServerConfiguration = A.Fake<ScimServerConfiguration>();
            UserRepository = A.Fake<IUserRepository>();
            PasswordManager = A.Fake<IManagePasswords>();
            PasswordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(true);
            
            A.CallTo(() => UserRepository.CreateUser(A<User>._))
                .ReturnsLazily(c =>
                {
                    var user = (User) c.Arguments[0];
                    user.Id = Guid.NewGuid().ToString("N");

                    return Task.FromResult(user);
                });

            var etagProvider = A.Fake<IResourceVersionProvider>();
            var canonicalizationService = A.Fake<DefaultCanonicalizationService>();
            _UserService = new UserService(
                ServerConfiguration,
                canonicalizationService,
                UserRepository, 
                PasswordManager,
                new UserValidatorFactory(UserRepository, PasswordComplexityVerifier, PasswordManager))
            {
                VersionProvider = etagProvider
            };
        };

        Because of = async () => Result = await _UserService.CreateUser(ClientUserDto).AwaitResponse().AsTask;

        protected static User ClientUserDto;

        protected static ScimServerConfiguration ServerConfiguration;

        protected static IUserRepository UserRepository;

        protected static IManagePasswords PasswordManager;

        protected static IVerifyPasswordComplexity PasswordComplexityVerifier;

        protected static IScimResponse<User> Result;

        private static IUserService _UserService;
    }
}