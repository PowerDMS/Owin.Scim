namespace Owin.Scim.Tests.Integration.ResourceTypes
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Extensions;

    using Machine.Specifications;
    using v2.Model;

    [Ignore("This shows the exception caused by application/xml")]
    public class when_retrieving_resource_types_error : using_a_scim_server
    {
        Because of = async () =>
        {
            var client = Server.HttpClient;
            client.DefaultRequestHeaders.Add("Accept", "application/xml");

            Response = await client
                .GetAsync("v2/resourcetypes/" + ResourceTypeName ?? string.Empty)
                .AwaitResponse()
                .AsTask;

            var content = await Response.Content.ReadAsStringAsync();
            await Response.DeserializeTo(() => ResourceTypes);
        };

        It should_list_defined_resource_types = () => ResourceTypes.ShouldNotBeEmpty();

        protected static string ResourceTypeName;

        protected static HttpResponseMessage Response;

        protected static IEnumerable<ResourceType> ResourceTypes;
    }
}