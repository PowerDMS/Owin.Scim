namespace Owin.Scim.Extensions
{
    using System.Text.RegularExpressions;

    using Model;

    public static class StringExtensions
    {
        private static readonly Regex _NamespaceScimVersionRegex =
            new Regex("(?:\\.)(v[0-9]+)(?:\\.)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string RemoveMultipleSpaces(this string value)
        {
            return Regex.Replace(value.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// Sets first letter to uppercase, and all remaining to lowercase
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UppercaseFirstCharacter(this string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
        }

        public static string LowercaseFirstCharacter(this string text)
        {
            return text.Substring(0, 1).ToLower() + text.Substring(1);
        }

        public static int NthIndexOf(this string haystack, char needle, int startIndex, int occurence)
        {
            int i = 1;
            int index = startIndex;

            while (i <= occurence && (index = haystack.IndexOf(needle, index + 1)) != -1)
            {
                if (i == occurence)
                    return index;

                i++;
            }

            return -1;
        }

        public static ScimVersion GetScimVersion(this string @namespace)
        {
            var result = _NamespaceScimVersionRegex.Match(@namespace);
            if (result.Success)
                return new ScimVersion(result.Groups[1].Value); // e.g. groups[] -> /v0/, v0

            return null;
        }
    }
}