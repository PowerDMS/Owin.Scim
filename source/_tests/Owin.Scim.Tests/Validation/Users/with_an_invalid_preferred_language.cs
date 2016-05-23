namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    public class with_an_invalid_preferred_language : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
                UserName = "daniel",
                PreferredLanguage = "invalid"
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}