namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Net;

    using Machine.Specifications;

    public class with_a_null_extension_value : when_creating_a_resource_with_a_required_schema_extension
    {
        Establish context = () =>
        {
            TenantDto = new Tenant();
            TenantDto.AddNullExtension(typeof(SalesForceExtension), CustomSchemas.SalesForceExtension);
        };

        It should_return_400 = () => Error.Status.ShouldEqual(HttpStatusCode.BadRequest);
    }
}