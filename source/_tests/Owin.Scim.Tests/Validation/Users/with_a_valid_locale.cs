namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_a_valid_locale : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser2
            {
                UserName = "daniel",
                Locale = "en-US"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}