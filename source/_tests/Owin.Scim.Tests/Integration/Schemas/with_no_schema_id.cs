namespace Owin.Scim.Tests.Integration.Schemas
{
    using System.Linq;

    using Machine.Specifications;

    public class with_no_schema_id : when_retrieving_schemas
    {
        It should_return_core_schemas = 
            () => 
                Schemas
                    .Select(schema => schema.Id)
                    .ShouldContain(
                        new []
                        {
                            "urn:ietf:params:scim:schemas:core:2.0:User",
                            "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
                            "urn:ietf:params:scim:schemas:core:2.0:Group",
                            "urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig",
                            "urn:ietf:params:scim:schemas:core:2.0:ResourceType",
                            "urn:ietf:params:scim:schemas:core:2.0:Schema"
                        });
    }
}