namespace Owin.Scim.Tests.Validation.Users
{
    using FakeItEasy;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_password : when_validating_a_new_user
    {
        Establish ctx = () =>
        {
            A.CallTo(() => PasswordComplexityVerifier.MeetsRequirements(A<string>._))
                .Returns(true);

            User = new User
            {
                UserName = "daniel",
                Password = "secret"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}