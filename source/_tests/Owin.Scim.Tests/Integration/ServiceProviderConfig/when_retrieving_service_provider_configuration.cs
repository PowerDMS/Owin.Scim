namespace Owin.Scim.Tests.Integration.ServiceProviderConfig
{
    using System.Collections.Generic;
    using System.Net.Http;

    using Extensions;

    using Machine.Specifications;
    
    using v2.Model;

    public class when_retrieving_service_provider_configuration : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .GetAsync("v2/serviceproviderconfig")
                .AwaitResponse()
                .AsTask;

            await Response.DeserializeTo(
                () => Configuration,
                () => JsonData);
        };

        It should_not_serialize_id = () => JsonData.ContainsKey("id").ShouldBeFalse();

        It should_list_authentication_schemes = () => Configuration.AuthenticationSchemes.ShouldNotBeEmpty();
        
        protected static HttpResponseMessage Response;

        protected static ServiceProviderConfiguration2 Configuration;

        protected static IDictionary<string, object> JsonData;
    }
}