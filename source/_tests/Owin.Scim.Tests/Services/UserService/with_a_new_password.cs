namespace Owin.Scim.Tests.Services.UserService
{
    using System.Threading.Tasks;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_new_password : when_updating_a_user
    {
        Establish context = () =>
        {
            A.CallTo(() => PasswordManager.CreateHash(A<string>._))
                .Returns("passhash");

            A.CallTo(() => PasswordComplexityVerifier.MeetsRequirements(A<string>._))
                .Returns(true);
            
            A.CallTo(() => UserRepository.GetUser(A<string>._))
                .ReturnsLazily(GetUserRecord);

            ClientUserDto = new User
            {
                Id = "id",
                UserName = "name",
                Password = "newpass"
            };
        };

        It should_hash_the_new_password = () => A.CallTo(() => PasswordManager.CreateHash(A<string>._)).MustHaveHappened();

        private static async Task<User> GetUserRecord()
        {
            return new User
            {
                Id = "id",
                UserName = "name",
                Password = "pass"
            };
        }
    }
}