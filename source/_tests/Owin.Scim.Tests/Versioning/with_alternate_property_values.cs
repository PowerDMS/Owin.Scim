namespace Owin.Scim.Tests.Versioning
{
    using Machine.Specifications;

    using Model.Users;

    public class with_alternate_property_values : when_generating_a_User_etags<User>
    {
        Establish ctx = () => User = new User();

        Because of = () =>
        {
            User.UserName = "daniel";
            User.DisplayName = "marc";

            User1ETag = User.CalculateVersion();

            User.UserName = "marc";
            User.DisplayName = "daniel";

            User2ETag = User.CalculateVersion();
        };

        It should_be_different_values = () => User1ETag.ShouldNotEqual(User2ETag);
    }
}