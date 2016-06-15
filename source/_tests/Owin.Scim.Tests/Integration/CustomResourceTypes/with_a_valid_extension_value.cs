namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Net;

    using Machine.Specifications;

    public class with_a_valid_extension_value : when_creating_a_resource_with_a_required_schema_extension
    {
        Establish context = () =>
        {
            TenantDto = new Tenant { Name = "Customer1" };
            TenantDto.AddExtension(new SalesForceExtension { CustomerIdentifier = "SFCustomerID" });
        };

        It should_return_201 = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);
    }
}