namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;

    using Microsoft.Owin;

    using Model;

    using NContext.Common;

    using Querying;

    public static class IReadableStringCollectionExtensions
    {
        public static ScimQueryOptions GetScimQueryOptions(this IReadableStringCollection collection, ScimServerConfiguration configuration)
        {
            var queryOptions = new ScimQueryOptions();

            queryOptions.Attributes = collection.GetValues("attributes")
                .ToMaybe()
                .Bind(attributes => new HashSet<string>(attributes.Distinct(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase).ToMaybe())
                .FromMaybe(new HashSet<string>());

            queryOptions.ExcludedAttributes = collection.GetValues("excludedAttributes")
                .ToMaybe()
                .Bind(attributes => new HashSet<string>(attributes.Distinct(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase).ToMaybe())
                .FromMaybe(new HashSet<string>());

            queryOptions.Filter = collection.Get("filter")
                .ToMaybe()
                .Bind(filter => new ScimFilter(configuration.ResourceExtensionSchemas, filter).Paths.MaybeSingle())
                .FromMaybe(null);

            queryOptions.SortBy = collection.Get("sortBy");

            queryOptions.SortOrder = collection.Get("sortOrder")
                .ToMaybe()
                .Bind(
                    sortOrder =>
                        (sortOrder.Equals("descending", StringComparison.OrdinalIgnoreCase)
                            ? SortOrder.Descending
                            : SortOrder.Ascending).ToMaybe())
                .FromMaybe(SortOrder.Ascending); // default

            queryOptions.StartIndex = collection.Get("startIndex")
                .ToMaybe()
                .Bind(index =>
                {
                    // The 1-based index of the first query result. A value less than 1 SHALL be interpreted as 1.

                    int indexInt = 1;
                    try { indexInt = Convert.ToInt32(index); } catch {}
                    
                    return (indexInt < 1 ? 1 : indexInt).ToMaybe();
                })
                .FromMaybe(1); // default

            queryOptions.Count = collection.Get("count")
                .ToMaybe()
                .Bind(count =>
                {
                    // Non-negative integer. Specifies the desired maximum number of query 
                    // results per page, e.g., 10. A negative value SHALL be interpreted as 
                    // "0". A value of "0" indicates that no resource indicates that no resource 
                    // except for "totalResults".

                    int countInt = 0;
                    try { countInt = Convert.ToInt32(count); } catch { }

                    return (countInt < 0 ? 0 : countInt).ToMaybe();
                })
                .FromMaybe(configuration.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter).MaxResults); // default

            return queryOptions;
        }
    }
}