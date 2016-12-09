using System.Net.Http;
using Machine.Specifications;
using Owin.Scim.Tests.Extensions;
using Owin.Scim.v2;
using Owin.Scim.v2.Model;

namespace Owin.Scim.Tests.Integration.ResourceTypes
{
    public class when_retrieving_resource_type_by_name : using_a_scim_server
    {
        private Establish context = () =>
        {
            ResourceTypeName = "User";
        };

        Because of = async () =>
        {
            var client = Server.HttpClient;
            client.DefaultRequestHeaders.Add("Accept", "application/scim+json");

            Response = await client
                        .GetAsync("v2/resourcetypes/" + ResourceTypeName)
                        .AwaitResponse()
                        .AsTask;

            var content = await Response.Content.ReadAsStringAsync();
            await Response.DeserializeTo(() => ResourceType);
        };

        It should_list_defined_resource_types = () => ResourceType.Schema.ShouldMatch(ScimConstantsV2.Schemas.User);

        protected static string ResourceTypeName;

        protected static HttpResponseMessage Response;

        protected static ResourceType ResourceType;
    }
}