namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_username_that_already_exists : when_validating_a_new_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._, A<string>._))
                .Returns(false);

            User = new User
            {
                UserName = "daniel"
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}