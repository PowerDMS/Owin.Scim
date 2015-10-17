namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_invalid_preferred_language : when_validating_a_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._, A<string>._))
                .Returns(true);

            User = new User
            {
                UserName = "daniel",
                PreferredLanguage = "invalid"
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}