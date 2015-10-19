namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_invalid_password : when_validating_a_new_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => PasswordComplexityVerifier.MeetsRequirements(A<string>._))
                .Returns(false);

            User = new User
            {
                UserName = "daniel",
                Password = "invalid"
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}