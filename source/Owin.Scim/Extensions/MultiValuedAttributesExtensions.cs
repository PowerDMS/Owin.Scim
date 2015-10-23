namespace Owin.Scim.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

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
    }
}