namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Net;

    using Machine.Specifications;

    public class with_no_extension : when_creating_a_resource_with_a_required_schema_extension
    {
        Establish context = () =>
        {
            TenantDto = new Tenant();
        };

        It should_return_400 = () => Error.Status.ShouldEqual(HttpStatusCode.BadRequest);
    }
}