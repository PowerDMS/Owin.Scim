namespace Owin.Scim.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Model;

    public static class MultiValuedAttributesExtensions
    {
        public static int GetMultiValuedAttributeCollectionVersion<T>(
            this IEnumerable<T> multiValuedAttributes)
            where T : MultiValuedAttribute
        {
            if (multiValuedAttributes == null || !multiValuedAttributes.Any())
                return 0;

            unchecked
            {
                int hash = 19;
                foreach (var mva in multiValuedAttributes)
                {
                    if (mva != null)
                        hash = hash * 31 + mva.CalculateVersion();
                }

                return hash;
            }
        }
    }
}