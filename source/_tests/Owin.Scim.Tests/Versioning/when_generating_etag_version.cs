namespace Owin.Scim.Tests.Versioning
{
    using Model.Users;

    public class when_generating_a_User_etags<TUser> where TUser : User
    {
        protected static string User1ETag;
        
        protected static string User2ETag;

        protected static TUser User;
    }
}