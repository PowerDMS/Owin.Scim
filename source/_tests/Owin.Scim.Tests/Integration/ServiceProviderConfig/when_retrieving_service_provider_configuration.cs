namespace Owin.Scim.Tests.Integration.ServiceProviderConfig
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Extensions;

    using Machine.Specifications;

    using Model;

    public class when_retrieving_service_provider_configuration : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .GetAsync("serviceproviderconfig")
                .AwaitResponse()
                .AsTask;

            await Response.DeserializeTo(
                () => Config,
                () => JsonData);
        };

        It should_not_serialize_id = () => JsonData.ContainsKey("id").ShouldBeFalse();

        It should_list_authentication_schemes = () => Config.AuthenticationSchemes.ShouldNotBeEmpty();
        
        protected static HttpResponseMessage Response;

        protected static ServiceProviderConfig Config;

        protected static IDictionary<string, object> JsonData;
    }
}