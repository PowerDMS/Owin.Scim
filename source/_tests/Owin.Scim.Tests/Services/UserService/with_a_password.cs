namespace Owin.Scim.Tests.Services.UserService
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_password : when_updating_a_user
    {
        Establish context = () =>
        {
            A.CallTo(() => PasswordManager.VerifyHash(A<string>._, A<string>._))
                .ReturnsLazily(c => c.Arguments[0].Equals(c.Arguments[1]));

            ClientUserDto = new User
            {
                Id = "id",
                UserName = "name",
                DisplayName = "Danny"
            };

            UserRecord = new User
            {
                Id = "id",
                UserName = "name",
                DisplayName = "Daniel",
                Password = "pass"
            };
        };

        It should_not_return_the_password_ever = () => Result.Password.ShouldBeNull();
    }
}