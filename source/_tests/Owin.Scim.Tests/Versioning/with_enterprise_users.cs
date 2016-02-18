namespace Owin.Scim.Tests.Versioning
{
    using Machine.Specifications;

    using Model.Users;

    public class with_enterprise_users : when_generating_a_User_etags<EnterpriseUser>
    {
        Establish ctx = () => User = new EnterpriseUser
        {
            Enterprise = new EnterpriseUserExtension()
        };

        Because of = () =>
        {
            User.UserName = "daniel";

            User1ETag = User.CalculateVersion();

            User.UserName = "daniel";
            User.Enterprise.Manager = new Manager { Value = "Chi Ho" };

            User2ETag = User.CalculateVersion();
        };

        It should_be_different_values = () => User1ETag.ShouldNotEqual(User2ETag);
    }
}