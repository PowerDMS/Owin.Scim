namespace Owin.Scim.Tests.Integration.Users
{
    using System.Threading;

    public static class UserNameUtility
    {
        private static int _UserCount;

        public static string GenerateUserName()
        {
            return string.Format("user{0}", Interlocked.Increment(ref _UserCount));
        }
    }
}