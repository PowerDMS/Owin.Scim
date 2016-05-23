namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    public class with_a_username : when_validating_a_new_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser
            {
                UserName = "daniel"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}