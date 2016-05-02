namespace Owin.Scim.Tests.Querying.ScimQueryOptions
{
    using System.IO;

    using Machine.Specifications;

    using Model;

    using Newtonsoft.Json;

    using Scim.Querying;

    using Serialization;

    public class with_request_body_parameters : when_querying_scim_resources
    {
        Establish context = () =>
        {
            QueryOptionsFactory = () =>
            {
                var json = 
                    @"{
                    ""attributes"": [""userName""],
                    ""excludedAttributes"": [""name.givenName""],
                    ""filter"": ""userName Eq \""john\"""",
                    ""sortBy"": ""title"",
                    ""sortOrder"": ""descending"",
                    ""startIndex"": 5,
                    ""count"": 50
                }";

                var serializer = new JsonSerializer();
                var reader = new JsonTextReader(new StringReader(json));
                serializer.Converters.Add(new ScimQueryOptionsConverter(ServerConfiguration));

                return serializer.Deserialize<ScimQueryOptions>(reader);
            };
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