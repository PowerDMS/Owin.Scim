namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_new_password : when_validating_an_existing_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => UserRepository.GetUser(A<string>._))
                .ReturnsLazily(() => new User
                {
                    Id = "id",
                    UserName = "daniel",
                    Password = "oldPass"
                });

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._, A<string>._))
                .Returns(true);

            A.CallTo(() => PasswordComplexityVerifier.MeetsRequirements(A<string>._))
                .Returns(true);

            A.CallTo(() => PasswordManager.VerifyHash(A<string>._, A<string>._))
                .ReturnsLazily(c => c.Arguments[0].Equals(c.Arguments[1]));

            User = new User
            {
                Id = "id",
                UserName = "daniel",
                Password = "newPass"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}