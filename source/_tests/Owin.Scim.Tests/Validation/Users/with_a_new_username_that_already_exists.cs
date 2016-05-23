namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_new_username_that_already_exists : when_validating_an_existing_user
    {
        Establish ctx = () =>
        {
            ExistingUserRecord = new ScimUser { UserName = "daniel" };

            A.CallTo(() => UserRepository.IsUserNameAvailable(A<string>._))
                .Returns(false);

            User = new ScimUser
            {
                UserName = "newdaniel"
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}