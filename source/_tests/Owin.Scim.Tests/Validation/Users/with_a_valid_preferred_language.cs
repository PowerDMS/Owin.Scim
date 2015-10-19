namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_preferred_language : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new User
            {
                UserName = "daniel",
                PreferredLanguage = "da, en-gb;q=0.8, en;q=0.7"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}