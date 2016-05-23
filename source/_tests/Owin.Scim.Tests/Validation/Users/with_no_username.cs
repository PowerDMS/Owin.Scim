namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    public class with_no_username : when_validating_a_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
            };
        };

        It should_be_invalid = () => ((bool)Result).ShouldEqual(false);
    }
}