namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_a_valid_preferred_language : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser2
            {
                UserName = "daniel",
                PreferredLanguage = "da, en-gb;q=0.8, en;q=0.7"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}