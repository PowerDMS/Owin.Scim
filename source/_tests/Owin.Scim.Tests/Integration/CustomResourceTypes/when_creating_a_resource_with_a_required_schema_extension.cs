﻿namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model;

    public class when_creating_a_resource_with_a_required_schema_extension : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync(
                    "tenants", 
                    TenantJson == null 
                        ? (HttpContent)new ScimObjectContent<Tenant>(TenantDto) 
                        : new StringContent(TenantJson, Encoding.UTF8, "application/scim+json"))
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

        protected static string TenantJson;

        protected static Tenant CreatedTenant;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }

    [Ignore("Bugfix coming soon.")]
    public class with_a_null_extension_value : when_creating_a_resource_with_a_required_schema_extension
    {
        Establish context = () =>
        {
            TenantJson = @"
                {
                  ""schemas"": [""" + CustomSchemas.Tenant + @""",""" + CustomSchemas.SalesForceExtension + @"""],
                  ""name"": ""Waking Venture"",
                  ""urn:custom:schemas:extensions:salesforce"": null,
                }";
        };

        It should_return_BadRequest = () => Error.Status.ShouldEqual(HttpStatusCode.BadRequest);
    }
}