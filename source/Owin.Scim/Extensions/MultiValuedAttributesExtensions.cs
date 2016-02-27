namespace Owin.Scim.Extensions
{
    using System.Collections.Generic;

    using Model;

    public static class MultiValuedAttributesExtensions
    {
        public static int GetMultiValuedAttributeCollectionVersion<T>(
            this IEnumerable<T> multiValuedAttributes)
            where T : MultiValuedAttribute
        {
            if (multiValuedAttributes == null)
                return 0;

            unchecked
            {
                int hash = 19;
                foreach (var mva in multiValuedAttributes)
                {
                    hash = hash * 31 + (mva?.CalculateVersion() ?? 0);
                }

                return hash;
            }
        }
    }
}