namespace Owin.Scim.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;

    using Model;

    using NContext.Extensions;

    public static class MultiValuedAttributesExtensions
    {
        public static void Canonicalize<T>(
            this IEnumerable<T> multiValuedAttributes,
            params CanonicalizationRule<T>[] canonicalizationRules)
            where T : MultiValuedAttribute
        {
            if (multiValuedAttributes == null || !multiValuedAttributes.Any()) return;

            var stateCache = new Dictionary<CanonicalizationRule<T>, object>();
            multiValuedAttributes.ForEach(attribute =>
            {
                if (attribute == null) return;

                canonicalizationRules.ForEach(rule =>
                {
                    if (!stateCache.ContainsKey(rule))
                        stateCache[rule] = null;

                    var state = stateCache[rule];
                    rule(attribute, ref state);
                    stateCache[rule] = state;
                });
            });
        }

        public static int GetMultiValuedAttributeCollectionETagHashCode<T>(
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
                    hash = hash * 31 + (mva?.GetETagHashCode() ?? 0);
                }

                return hash;
            }
        }
    }
}