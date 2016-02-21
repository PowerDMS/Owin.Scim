namespace Owin.Scim.Tests.Integration.ResourceTypes
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Extensions;

    using Machine.Specifications;

    using Model;

    public class when_retrieving_resource_types : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .GetAsync("resourcetypes/" + ResourceTypeName ?? string.Empty)
                .AwaitResponse()
                .AsTask;

            await Response.DeserializeTo(() => ResourceTypes);
        };

        It should_list_defined_resource_types = () => ResourceTypes.ShouldNotBeEmpty();

        protected static string ResourceTypeName;

        protected static HttpResponseMessage Response;

        protected static IEnumerable<ResourceType> ResourceTypes;
    }
}