namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;

    public class when_creating_a_resource_with_a_required_schema_extension : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync("tenants", new ScimObjectContent<Tenant>(TenantDto))
                .AwaitResponse()
                .AsTask;

            CreatedTenant = Response.StatusCode == HttpStatusCode.Created
                ? await Response.Content.ScimReadAsAsync<Tenant>().AwaitResponse().AsTask
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? await Response.Content.ScimReadAsAsync<ScimError>().AwaitResponse().AsTask
                : null;
        };

        protected static Tenant TenantDto;

        protected static Tenant CreatedTenant;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}