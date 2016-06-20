namespace Owin.Scim.Tests.Services.UserService.Update
{
    using System.Threading.Tasks;

    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_a_new_password : when_updating_a_user
    {
        Establish context = () =>
        {
            A.CallTo(() => PasswordManager.CreateHash(A<string>._))
                .Returns("passhash");

            A.CallTo(() => PasswordManager.MeetsRequirements(A<string>._))
                .Returns(true);

            A.CallTo(() => PasswordManager.PasswordIsDifferent(A<string>._, A<string>._))
                .Returns(true);
            
            A.CallTo(() => UserRepository.GetUser(A<string>._))
                .ReturnsLazily(GetUserRecord);

            ClientUserDto = new ScimUser2
            {
                Id = "id",
                UserName = "name",
                Password = "newpass"
            };
        };

        It should_hash_the_new_password = () => A.CallTo(() => PasswordManager.CreateHash(A<string>._)).MustHaveHappened();

        private static Task<ScimUser> GetUserRecord()
        {
            return Task.FromResult<ScimUser>(
                new ScimUser2
                {
                    Id = "id",
                    UserName = "name",
                    Password = "pass"
                });
        }
    }
}