namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_password : when_validating_a_new_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => PasswordManager.MeetsRequirements(A<string>._))
                .Returns(true);

            User = new ScimUser
            {
                UserName = "daniel",
                Password = "secret"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}