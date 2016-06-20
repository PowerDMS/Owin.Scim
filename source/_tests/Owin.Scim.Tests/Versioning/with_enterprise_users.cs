namespace Owin.Scim.Tests.Versioning
{
    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_enterprise_users : when_generating_a_User_etags<ScimUser>
    {
        Establish ctx = () => User = new ScimUser2();

        Because of = () =>
        {
            User.UserName = "daniel";

            User1ETag = User.CalculateVersion();

            User.UserName = "daniel";
            User.Extension<EnterpriseUser2Extension>().Manager = new Manager { Value = "Chi Ho" };

            User2ETag = User.CalculateVersion();
        };

        It should_be_different_values = () => User1ETag.ShouldNotEqual(User2ETag);
    }
}