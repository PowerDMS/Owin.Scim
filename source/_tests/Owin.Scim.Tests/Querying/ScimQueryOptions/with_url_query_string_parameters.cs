namespace Owin.Scim.Tests.Querying.ScimQueryOptions
{
    using System.Collections.Generic;

    using Machine.Specifications;

    using Microsoft.Owin;

    using Model;

    using Scim.Extensions;

    public class with_url_query_string_parameters : when_querying_scim_resources
    {
        Establish context = () =>
        {
            QueryOptionsFactory = () =>
                new OwinRequest(
                    new Dictionary<string, object>
                    {
                        {
                            "owin.RequestQueryString",
                            "attributes=userName&excludedAttributes=name.givenName&filter=userName Eq \"john\"&sortBy=title&sortOrder=descending&startIndex=5&count=50"
                        }
                    })
                    .Query
                    .GetScimQueryOptions(ServerConfiguration);
        };

        It should_set_attibutes = () => QueryOptions.Attributes.ShouldContain("userName");

        It should_set_excludedAttributes = () => QueryOptions.ExcludedAttributes.ShouldContain("name.givenName");

        It should_set_filter = () => QueryOptions.Filter.Filter.ShouldEqual("userName Eq \"john\"");

        It should_set_sortBy = () => QueryOptions.SortBy.ShouldEqual("title");

        It should_set_sortOrder = () => QueryOptions.SortOrder.ShouldEqual(SortOrder.Descending);

        It should_set_startIndex = () => QueryOptions.StartIndex.ShouldEqual(5);

        It should_set_count = () => QueryOptions.Count.ShouldEqual(50);
    }
}