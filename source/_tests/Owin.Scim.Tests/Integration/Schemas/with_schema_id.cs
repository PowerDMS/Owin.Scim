namespace Owin.Scim.Tests.Integration.Schemas
{
    using System.Linq;

    using Machine.Specifications;

    public class with_schema_id : when_retrieving_schemas
    {
        Establish context = () => SchemaId = "urn:ietf:params:scim:schemas:core:2.0:ResourceType";

        It should_return_specific_schema =
            () =>
                Schemas
                    .Single()
                    .Id
                    .ShouldEqual("urn:ietf:params:scim:schemas:core:2.0:ResourceType");
    }
}