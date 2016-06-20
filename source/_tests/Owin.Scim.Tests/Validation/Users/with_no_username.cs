namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_no_username : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser2
            {
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}