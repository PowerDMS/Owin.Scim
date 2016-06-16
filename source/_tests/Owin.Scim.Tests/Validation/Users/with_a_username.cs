namespace Owin.Scim.Tests.Validation.Users
{
    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_a_username : when_validating_a_new_user
    {
        Establish ctx = () =>
        {
            User = new ScimUser2
            {
                UserName = "daniel"
            };
        };

        It should_be_valid = () => ((bool)Result).ShouldEqual(true);
    }
}