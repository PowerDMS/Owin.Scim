namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections;

    public static class IEnumerableExtensions
    {
        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException("enumerable");

            int count = 0;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                count++;
            }

            return count;
        }
    }
}