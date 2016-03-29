namespace Owin.Scim.Tests.Services.UserService.Update
{
    using System.Threading.Tasks;

    using Canonicalization;

    using Configuration;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    using Repository;

    using Scim.Security;
    using Scim.Services;

    using Validation.Users;

    public class when_updating_a_user
    {
        Establish context = () =>
        {
            ServerConfiguration = A.Fake<ScimServerConfiguration>();
            UserRepository = A.Fake<IUserRepository>();
            GroupRepository = A.Fake<IGroupRepository>();
            PasswordManager = A.Fake<IManagePasswords>();
            
            A.CallTo(() => UserRepository.UpdateUser(A<User>._))
                .ReturnsLazily(c => Task.FromResult((User)c.Arguments[0]));

            var etagProvider = A.Fake<IResourceVersionProvider>();
            var canonicalizationService = A.Fake<DefaultCanonicalizationService>();
            _UserService = new UserService(
                ServerConfiguration,
                canonicalizationService,
                UserRepository,
                GroupRepository,
                PasswordManager,
                new UserValidatorFactory(UserRepository, PasswordManager))
            {
                VersionProvider = etagProvider
            };
        };

        Because of = async () => Result = await _UserService.UpdateUser(ClientUserDto).AwaitResponse().AsTask;

        protected static User ClientUserDto;

        protected static ScimServerConfiguration ServerConfiguration;

        protected static IUserRepository UserRepository;

        protected static IGroupRepository GroupRepository;

        protected static IManagePasswords PasswordManager;

        protected static IScimResponse<User> Result;

        private static IUserService _UserService;
    }
}