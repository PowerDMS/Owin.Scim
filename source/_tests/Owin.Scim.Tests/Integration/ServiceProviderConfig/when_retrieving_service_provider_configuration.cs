namespace Owin.Scim.Tests.Integration.ServiceProviderConfig
{
    using System.Net;
    using System.Net.Http;

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

            if (Response.StatusCode == HttpStatusCode.OK)
                Config = await Response.Content.ReadAsAsync<ServiceProviderConfig>();
        };

        It should_list_authentication_schemes = () => Config.AuthenticationSchemes.ShouldNotBeEmpty();
        
        protected static HttpResponseMessage Response;

        protected static ServiceProviderConfig Config;
    }
}