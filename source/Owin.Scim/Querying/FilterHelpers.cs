namespace Owin.Scim.Querying
{
    using System;
    using System.Collections;
    using System.Reflection;

    using Extensions;

    public class FilterHelpers
    {
        public static bool StartsWith(string haystack, string needle)
        {
            if (haystack == null || needle == null) return false;

            return haystack.StartsWith(needle, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWith(string haystack, string needle)
        {
            if (haystack == null || needle == null) return false;

            return haystack.EndsWith(needle, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(string haystack, string needle)
        {
            if (haystack == null || needle == null) return false;

            return haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static bool IsPresent<TResource>(TResource resource, PropertyInfo property)
        {
            if (resource == null || property == null) return false;

            var value = property.GetValue(resource);
            if (value == null) return false;

            var valueType = value.GetType();
            if (valueType == typeof(string))
            {
                return !string.IsNullOrWhiteSpace(value as string);
            }

            if (valueType.IsNonStringEnumerable())
            {
                var enumerable = (IEnumerable)value;
                var enumerator = enumerable.GetEnumerator();
                return enumerator.MoveNext();
            }

            return true;
        }
    }
}