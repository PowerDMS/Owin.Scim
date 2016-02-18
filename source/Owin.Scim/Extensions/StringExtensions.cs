namespace Owin.Scim.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string RemoveMultipleSpaces(this string value)
        {
            return Regex.Replace(value.Trim(), @"\s+", " ");
        }
    }
}