namespace Owin.Scim.Tests.Integration.Groups
{
    using System.Threading;

    public static class GroupNameUtility
    {
        private static int _GroupCount;

        public static string GenerateGroupName()
        {
            return string.Format("group{0}", Interlocked.Increment(ref _GroupCount));
        }
    }
}