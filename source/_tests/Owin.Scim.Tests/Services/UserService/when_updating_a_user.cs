namespace Owin.Scim.Tests.Services.UserService
{
    using System.Threading.Tasks;

    using AutoMapper;

    using FakeItEasy;

    using Machine.Specifications;

    using Mappings;

    using Model.Users;

    using Repository;

    using Scim.Services;
    using Scim.Validation.Users;

    using Security;

    public abstract class when_updating_a_user
    {
        Establish context = () =>
        {
            Mapper.Initialize(c => new UserMapping().Configure(Mapper.Configuration));

            UserRepository = A.Fake<IUserRepository>();
            PasswordManager = A.Fake<IManagePasswords>();
            PasswordComplexityVerifier = A.Fake<IVerifyPasswordComplexity>();

            A.CallTo(() => UserRepository.GetUser(A<string>._))
                .ReturnsLazily(() => UserRecord);

            A.CallTo(() => UserRepository.UpdateUser(A<User>._))
                .ReturnsLazily(c => Task.FromResult((User)c.Arguments[0]));

            _UserService = new UserService(
                UserRepository, 
                PasswordManager, 
                new UserValidator(UserRepository, PasswordComplexityVerifier, PasswordManager));
        };

        Because of = async () => Result = await _UserService.UpdateUser(ClientUserDto).AwaitResponse().AsTask;

        protected static User ClientUserDto;

        protected static IUserRepository UserRepository;

        protected static IManagePasswords PasswordManager;

        protected static IVerifyPasswordComplexity PasswordComplexityVerifier;

        protected static User Result;

        protected static User UserRecord;

        private static IUserService _UserService;
    }
}